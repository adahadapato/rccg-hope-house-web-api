using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Infrastructure.Identity;

namespace RccgHopeHouse.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeds initial reference data on startup. Idempotent — safe to run every
/// time the app starts; only inserts records that don't already exist.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, 
                                        UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager,
                                        IConfiguration configuration, CancellationToken ct = default)
    {
        var theme2026 = await SeedThemeOfTheYearAsync(context, ct);
        await SeedPastorPostsAsync(context, theme2026, ct);
        await SeedChurchInfoAsync(context, ct);
        await SeedChurchServicesAsync(context, ct);
        await SeedServiceBroadcastsAsync(context, ct);
        await SeedGivingTypesAsync(context, ct);
        await SeedAdminAccountAsync(userManager, roleManager, configuration);
    }

    /// <summary>
    /// Seeds the required roles and one real Admin account, using credentials
    /// from configuration (User Secrets locally, environment variables/secrets
    /// manager in deployed environments) — never hardcoded. Idempotent: skips
    /// role/user creation if they already exist.
    /// </summary>
    private static async Task SeedAdminAccountAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration)
    {
        Console.WriteLine("=== SeedAdminAccountAsync: STARTED ===");

        string[] roles = ["Admin", "ContentEditor", "MediaManager", "PrayerTeam"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"=== Created role '{role}': Succeeded={roleResult.Succeeded} ===");
                if (!roleResult.Succeeded)
                    Console.WriteLine($"=== Role errors: {string.Join("; ", roleResult.Errors.Select(e => e.Description))} ===");
            }
        }

        var adminEmail = configuration["SeedData:AdminEmail"];
        var adminPassword = configuration["SeedData:AdminPassword"];

        Console.WriteLine($"=== AdminEmail from config: '{adminEmail}' ===");
        Console.WriteLine($"=== AdminPassword is set: {!string.IsNullOrWhiteSpace(adminPassword)} ===");

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException(
                "SeedData:AdminEmail and SeedData:AdminPassword must be configured.");
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        Console.WriteLine($"=== Existing admin found: {existingAdmin != null} ===");
        if (existingAdmin != null) return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        Console.WriteLine($"=== CreateAsync Succeeded: {result.Succeeded} ===");
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"=== CreateAsync errors: {errors} ===");
            throw new InvalidOperationException($"Failed to seed admin account: {errors}");
        }

        await userManager.AddToRoleAsync(admin, "Admin");
        Console.WriteLine("=== SeedAdminAccountAsync: COMPLETED SUCCESSFULLY ===");
    }


    /// <summary>
    /// Seeds the initial giving types used by the Give Online feature.
    ///
    /// Giving types are stored in the database rather than represented by
    /// an enum so that they can later be managed through the admin interface
    /// without requiring a code change or redeployment.
    ///
    /// The seeding process is idempotent: each giving type is checked
    /// individually before being inserted, allowing new giving types to be
    /// added to the seed list in future without duplicating existing records.
    /// </summary>
    private static async Task SeedGivingTypesAsync(
        ApplicationDbContext context,
        CancellationToken ct)
    {
        var givingTypes = new[]
        {
        GivingType.Create(
            name: "Tithe",
            displayOrder: 1,
            description: "Regular tithe giving."),

        GivingType.Create(
            name: "Offering",
            displayOrder: 2,
            description: "General church offering."),

        GivingType.Create(
            name: "Seed Offering",
            displayOrder: 3,
            description: "Giving offered as a seed of faith."),

        GivingType.Create(
            name: "Thanksgiving Offering",
            displayOrder: 4,
            description: "Giving offered in thanksgiving to God."),

        GivingType.Create(
            name: "Building Fund",
            displayOrder: 5,
            description: "Giving towards church building and development projects."),

        GivingType.Create(
            name: "Mission / Evangelism",
            displayOrder: 6,
            description: "Giving towards missions and evangelism activities."),

        GivingType.Create(
            name: "Welfare",
            displayOrder: 7,
            description: "Giving towards welfare and support for those in need."),

        GivingType.Create(
            name: "Special Offering",
            displayOrder: 8,
            description: "Giving towards special church programmes or purposes."),

        GivingType.Create(
            name: "Other",
            displayOrder: 9,
            description: "Giving for another purpose not listed above.")
    };

        foreach (var givingType in givingTypes)
        {
            var exists = await context.GivingTypes
                .AnyAsync(g => g.Name == givingType.Name, ct);

            if (exists)
                continue;

            await context.GivingTypes.AddAsync(givingType, ct);
        }

        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Seeds one current-month broadcast per HQ service category (Holy
    /// Communion, Holy Ghost Service, Thanksgiving Service). Only the Holy
    /// Ghost Service entry uses a real, confirmed YouTube URL
    /// (https://www.youtube.com/watch?v=FAgcFBNHYPk); the other two use
    /// clearly-marked placeholder video IDs that MUST be replaced with real
    /// URLs via the admin API (PUT /api/service-broadcasts/admin/{id}) before
    /// this goes live — otherwise their thumbnails/links will be broken.
    /// </summary>
    private static async Task SeedServiceBroadcastsAsync(ApplicationDbContext context, CancellationToken ct)
    {
        var exists = await context.ServiceBroadcasts.AnyAsync(ct);
        if (exists) return;

        var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var broadcasts = new[]
{
    ServiceBroadcast.Create(
        category: ServiceCategory.HolyGhostService,
        title: "PASTOR E.A ADEBOYE",
        youtubeUrl: "https://www.youtube.com/watch?v=FAgcFBNHYPk",
        serviceMonth: thisMonth,
        description: $"{thisMonth:MMMM yyyy} Holy Ghost Service",
        theme: "Divine Faithfulness"),

    ServiceBroadcast.Create(
        category: ServiceCategory.HolyCommunion,
        title: "PASTOR E.A ADEBOYE",
        youtubeUrl: "9sjD8MINjSo", // ⚠️ PLACEHOLDER
        serviceMonth: thisMonth,
        description: $"{thisMonth:MMMM yyyy} Holy Communion Service"),
        // No theme badge for Holy Communion — matches original screenshot, which showed no badge on that card.

    ServiceBroadcast.Create(
        category: ServiceCategory.ThanksgivingService,
        title: "PASTOR E.A ADEBOYE",
        youtubeUrl: "RXgNUBev7Ko", // ⚠️ PLACEHOLDER
        serviceMonth: thisMonth,
        description: $"{thisMonth:MMMM yyyy} Thanksgiving Service",
        theme: "Divine Partnership")
};

        await context.ServiceBroadcasts.AddRangeAsync(broadcasts, ct);
        await context.SaveChangesAsync(ct);
    }


    /// <summary>
    /// Seeds the ten regular/monthly church services shown across the live
    /// site (Hero.tsx, RegularServices.tsx, MonthlyServices.tsx), so future
    /// schedule changes can be made through the admin API instead of a code
    /// deploy.
    /// NOTE: End of Month Vigil and Holy Ghost Service cross midnight
    /// (StartTime "later" than EndTime). This is stored as-is by convention —
    /// any consuming code (frontend or otherwise) must interpret EndTime &lt;
    /// StartTime as "ends the following day."
    /// </summary>
    private static async Task SeedChurchServicesAsync(ApplicationDbContext context, CancellationToken ct)
    {
        var exists = await context.ChurchServices.AnyAsync(ct);
        if (exists) return;

        var services = new List<ChurchService>
    {
        // ===== Regular local services (RegularServices.tsx) =====
        ChurchService.Create(
            name: "Sunday School",
            category: ServiceCategory.SundaySchool,
            dayOfWeek: DayOfWeek.Sunday,
            startTime: new TimeSpan(10, 0, 0),
            endTime: new TimeSpan(11, 0, 0),
            recurrence: RecurrencePattern.Weekly,
            isLocal: true),

        ChurchService.Create(
            name: "Worship Service",
            category: ServiceCategory.WorshipService,
            dayOfWeek: DayOfWeek.Sunday,
            startTime: new TimeSpan(11, 0, 0),
            endTime: new TimeSpan(12, 40, 0),
            description: "Physical Service",
            recurrence: RecurrencePattern.Weekly,
            isLocal: true),

        ChurchService.Create(
            name: "Thanksgiving Sunday",
            category: ServiceCategory.ThanksgivingService,
            dayOfWeek: DayOfWeek.Sunday,
            startTime: new TimeSpan(11, 0, 0),
            endTime: new TimeSpan(12, 45, 0),
            description: "First Sunday of every month — local in-house service",
            recurrence: RecurrencePattern.FirstOfMonth,
            isLocal: true),

        ChurchService.Create(
            name: "Fasting and Prayer Day",
            category: ServiceCategory.WednesdayPrayer,
            dayOfWeek: DayOfWeek.Wednesday,
            startTime: new TimeSpan(19, 0, 0),
            endTime: new TimeSpan(19, 30, 0),
            description: "Online Prayer",
            recurrence: RecurrencePattern.Weekly,
            isLocal: true),

        ChurchService.Create(
            name: "End of Month Vigil",
            category: ServiceCategory.LastFridayVigil,
            dayOfWeek: DayOfWeek.Friday,
            startTime: new TimeSpan(22, 0, 0),
            endTime: new TimeSpan(1, 0, 0),
            description: "Last Friday of the Month",
            recurrence: RecurrencePattern.LastOfMonth,
            isLocal: true),

        ChurchService.Create(
            name: "Evangelism",
            category: ServiceCategory.Evangelism,
            dayOfWeek: DayOfWeek.Saturday,
            startTime: new TimeSpan(13, 0, 0),
            endTime: new TimeSpan(14, 0, 0),
            description: "Every Fortnight Saturdays",
            recurrence: RecurrencePattern.Fortnightly,
            isLocal: true),

        ChurchService.Create(
            name: "House Fellowship",
            category: ServiceCategory.HouseFellowship,
            dayOfWeek: DayOfWeek.Sunday,
            startTime: new TimeSpan(18, 0, 0),
            endTime: new TimeSpan(19, 0, 0),
            description: "Except 1st Sunday of the month",
            recurrence: RecurrencePattern.Weekly,
            isLocal: true),

        // ===== HQ broadcast monthly services (MonthlyServices.tsx) =====
        ChurchService.Create(
            name: "Holy Communion",
            category: ServiceCategory.HolyCommunion,
            dayOfWeek: DayOfWeek.Wednesday,
            startTime: new TimeSpan(18, 0, 0),
            endTime: new TimeSpan(20, 0, 0),
            description: "Broadcast from RCCG HQ, Lagos",
            recurrence: RecurrencePattern.FirstOfMonth,
            isLocal: false),

        ChurchService.Create(
            name: "Holy Ghost Service",
            category: ServiceCategory.HolyGhostService,
            dayOfWeek: DayOfWeek.Friday,
            startTime: new TimeSpan(22, 0, 0),
            endTime: new TimeSpan(0, 0, 0),
            description: "Broadcast from RCCG HQ, Lagos",
            recurrence: RecurrencePattern.FirstOfMonth,
            isLocal: false),

        ChurchService.Create(
            name: "Thanksgiving Service",
            category: ServiceCategory.ThanksgivingService,
            dayOfWeek: DayOfWeek.Sunday,
            startTime: new TimeSpan(9, 0, 0),
            endTime: new TimeSpan(12, 0, 0),
            description: "Broadcast from RCCG HQ, Lagos",
            recurrence: RecurrencePattern.FirstOfMonth,
            isLocal: false)
    };

        // Zoom/Location details, set via UpdateSchedule() since Create() doesn't
        // accept them directly.
        var fastingPrayer = services[3];
        fastingPrayer.UpdateSchedule(fastingPrayer.StartTime, fastingPrayer.EndTime, "Online", "833 483 0396", null);

        var houseFellowship = services[6];
        houseFellowship.UpdateSchedule(houseFellowship.StartTime, houseFellowship.EndTime, "Online", "833 483 0396", "333");

        for (int i = 0; i < services.Count; i++)
            services[i].SetDisplayOrder(i);

        await context.ChurchServices.AddRangeAsync(services, ct);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Seeds the church's contact profile and About-section content, matching
    /// the real content already on the site (About.tsx / Contact.tsx), so the
    /// database becomes the single source of truth for it going forward.
    /// </summary>
    private static async Task SeedChurchInfoAsync(ApplicationDbContext context, CancellationToken ct)
    {
        var exists = await context.ChurchInfo.AnyAsync(ct);
        if (exists) return;

        var churchInfo = ChurchInfo.Create(
            addressLine1: "123 Hope Street, Grace Avenue",
            city: "London",
            country: "United Kingdom",
            parishName: "RCCG Hope House Parish",
            establishedYear: 2012,
            tagline: "WHO WE ARE",
            aboutLead: "Located in Burnt Oak, Edgware, Greater London, we are a multi-cultural, evangelical ministry rooted in faith, love, and community.",
            aboutText: "Established in 2012, our church believes that everyone is important to God and should be treated with the care and love of Christ. We are building a sanctuary where faith grows, lives are transformed, and believers find a true spiritual home.",
            multiCulturalStat: "100%",
            addressLine2: null,
            postCode: "SW1A 1AA"
        );

        churchInfo.AddContactMethod(ChurchContactMethod.Create(
            churchInfo.Id, ContactMethodType.Phone, "+44 20 1234 5678", "Main Office", displayOrder: 0));

        churchInfo.AddContactMethod(ChurchContactMethod.Create(
            churchInfo.Id, ContactMethodType.Email, "info@rccghopehouse.org.uk", "General Enquiries", displayOrder: 1));

        await context.ChurchInfo.AddAsync(churchInfo, ct);
        await context.SaveChangesAsync(ct);
    }
    private static async Task<ThemeOfTheYear> SeedThemeOfTheYearAsync(ApplicationDbContext context, CancellationToken ct)
    {
        var existing = await context.ThemeOfTheYear.FirstOrDefaultAsync(t => t.Year == 2026, ct);
        if (existing != null) return existing;

        var theme2026 = ThemeOfTheYear.Create(
            year: 2026,
            themeTitle: "A Brand New Beginning",
            scriptureText: "Forget the former things; do not dwell on the past. See, I am doing a new thing! Now it springs up; do you not perceive it? I am making a way in the wilderness and streams in the wasteland.",
            scriptureReference: "Isaiah 43:18-19 (NIV)",
            primaryDescription: "According to prophecy for the year 2026 as declared by our father in the Lord (Daddy GO), we are in a season of a Brand-New beginning. To God be all the glory.",
            secondaryDescription: "More so, in agreement with the above text, the Lord is set to do a new thing. Let us put the ugly past behind us and be expectant for brand new things from the Lord, by living a brand-new life of complete obedience in holy living, good works, in giving of praises, thanksgiving and supplications.",
            callToActionText: "Join us as we embark on this transformative journey!"
        );

        await context.ThemeOfTheYear.AddAsync(theme2026, ct);
        await context.SaveChangesAsync(ct);
        return theme2026;
    }

    /// <summary>
    /// Seeds four articles under the 2026 "Brand New Beginning" theme: Joseph
    /// (migrated from the original hardcoded frontend content), David, Esther,
    /// and Ruth. Each is checked for existence individually so re-running
    /// seeding after adding new topics doesn't duplicate the ones already
    /// there.
    /// </summary>
    private static async Task SeedPastorPostsAsync(ApplicationDbContext context, ThemeOfTheYear theme, CancellationToken ct)
    {
        await SeedPostIfMissingAsync(context, theme, BuildJosephPost(theme.Id), ct);
        await SeedPostIfMissingAsync(context, theme, BuildDavidPost(theme.Id), ct);
        await SeedPostIfMissingAsync(context, theme, BuildEstherPost(theme.Id), ct);
        await SeedPostIfMissingAsync(context, theme, BuildRuthPost(theme.Id), ct);
    }

    private static async Task SeedPostIfMissingAsync(
    ApplicationDbContext context, ThemeOfTheYear theme, PastorPost post, CancellationToken ct)
    {
        var exists = await context.PastorPosts.AnyAsync(
            p => p.ThemeOfTheYearId == theme.Id && p.Title == post.Title, ct);
        if (exists) return;

        // Joseph is pinned so it reliably appears first in the feed (pinned posts
        // sort ahead of everything else) — it's the entry point into the theme;
        // the other topics (David, Esther, Ruth) are reachable via the sibling
        // popup once a reader opens Joseph's article.
        if (post.Title == "A Brand New Beginning – Joseph")
            post.Pin();

        post.Publish();
        await context.PastorPosts.AddAsync(post, ct);
        await context.SaveChangesAsync(ct);
    }

    // ============================================================
    // JOSEPH — migrated from the original hardcoded PastorsCorner.tsx content
    // ============================================================
    private static PastorPost BuildJosephPost(Guid themeId)
    {
        var structuredContent = new StructuredContentDto(
            FirstPoints: new List<FirstPointDto>
            {
                new(
                    Title: "God is Present in Every Location:",
                    Content: "Whether in the pit, Potiphar's house, or the prison, the Bible notes that \"the Lord was with Joseph\". This assures us that we are never alone in our struggles.",
                    Bullets: null),
                new(
                    Title: "Adversity is Preparation:",
                    Content: "Joseph's years in prison were not wasted time, but rather leadership training that prepared him for managing Egypt.",
                    Bullets: null),
                new(
                    Title: "God Turns Evil into Good (\"But God\")",
                    Content: null,
                    Bullets: new List<string>
                    {
                        "The Lesson: Regardless of the evil intentions or negative circumstances meant to destroy you, God has the power to orchestrate those same situations for your ultimate good and his glory.",
                        "Joseph's Example: Joseph explicitly told his brothers, \"You intended to harm me, but God intended it all for good\" (Gen. 50:20).",
                        "Application: When dealing with betrayal, injustice, or misfortune, believe that God is working behind the scenes to turn the situation into a blessing."
                    }),
                new(
                    Title: "The \"New Beginning\" is Often Larger than our Own Dream.",
                    Content: null,
                    Bullets: new List<string>
                    {
                        "God Gives a Bigger Dream: Joseph likely dreamed of a comfortable life, but God gave him a role in saving the known world.",
                        "God Uses Pain for Purpose: What others meant for evil, God used for good, transforming a \"rags to riches\" story into a, \"slave to savior\" story."
                    })
            },
            PrefaceSection: new PrefaceSectionDto(
                MainHeading: "lessons we can learn from God who gave a brand new beginning to Joseph in Genesis",
                Preamble: "The story of Joseph in Genesis (chapters 37-50) is a powerful narrative of divine providence, showing how God can take a person from the lowest pit to the highest palace. God's act of giving Joseph a \"brand new beginning\"—transforming him from a forgotten prisoner into the ruler of Egypt—offers profound lessons on faith, character, and trusting God's timing.",
                SubHeading: "Here are the key lessons we can learn from Joseph's new beginning:"
            ),
            DetailedLessons: new List<DetailedLessonDto>
            {
                new(
                    Title: "Character Matters More Than Circumstances",
                    Bullets: new List<string>
                    {
                        "The Lesson: God is less interested in our comfort and more interested in our character. He allows trials to build integrity, resilience, and faith.",
                        "Joseph's Example: In both Potiphar's house and the prison, Joseph acted with integrity, refusing to sin against God, even when it cost him his freedom.",
                        "Application: Maintain your integrity, even when no one is watching, and even when doing the right thing leads to temporary suffering."
                    }),
                new(
                    Title: "God's Timing is Perfect",
                    Bullets: new List<string>
                    {
                        "The Lesson: A new beginning often requires waiting. God's timeline is rarely our own, but his delays are designed to prepare us for the responsibility of the promotion.",
                        "Joseph's Example: Joseph spent 13 long years in slavery and prison before becoming prime minister at age 30.",
                        "Application: Do not become weary in well-doing; wait patiently for God's appointed time, knowing that he has not forgotten you."
                    }),
                new(
                    Title: "Forgiveness is the Key to Freedom",
                    Bullets: new List<string>
                    {
                        "The Lesson: Holding onto bitterness locks you in the past, but true forgiveness releases you to embrace the new beginning God has provided.",
                        "Joseph's Example: Instead of seeking revenge, Joseph forgave his brothers, recognizing that his suffering was part of a larger plan to save many lives.",
                        "Application: Release those who have hurt you, and focus on the purpose God has for you rather than the pain others caused you."
                    }),
                new(
                    Title: "God's Presence is Your Real Success",
                    Bullets: new List<string>
                    {
                        "The Lesson: True prosperity is not determined by material possessions, but by the presence of God in your life, regardless of whether you are in a prison or a palace.",
                        "Joseph's Example: \"The Lord was with Joseph, and he was a successful man\" (Gen. 39:2) — this was said while he was a slave in Potiphar's house.",
                        "Application: Focus on your relationship with God rather than your circumstances, knowing that his presence makes you successful in his eyes."
                    })
            }
        );

        return PastorPost.Create(
            title: "A Brand New Beginning – Joseph",
            content: "The story of Joseph in Genesis (chapters 37–50) provides profound lessons on how God can take a life marked by betrayal, slavery, and imprisonment and give it a 'brand new beginning' as a ruler.",
            category: PostCategory.Devotional,
            themeOfTheYearId: themeId,
            authorName: "Pastor",
            excerpt: "The story of Joseph in Genesis (chapters 37–50) provides profound lessons on how God can take a life marked by betrayal, slavery, and imprisonment and give it a 'brand new beginning' as a ruler.",
            introHeading: "INTRODUCTION",
            introText: "The story of Joseph in Genesis (chapters 37–50) provides profound lessons on how God can take a life marked by betrayal, slavery, and imprisonment and give it a \"brand new beginning\" as a ruler. Joseph's journey from the pit to the palace teaches that God works behind the scenes, transforming evil into good, and that faithfulness in adversity leads to divine promotion.",
            structuredContentJson: StructuredContentDto.Serialize(structuredContent),
            closingText: "With great joy, we welcome you all again into another year themed A Brand New Beginning.",
            bibleReference: "GENESIS 37-50"
        );
    }

    // ============================================================
    // DAVID — from the sheep pasture to the anointed king
    // ============================================================
    private static PastorPost BuildDavidPost(Guid themeId)
    {
        var structuredContent = new StructuredContentDto(
            FirstPoints: new List<FirstPointDto>
            {
                new(
                    Title: "God Sees What Man Overlooks:",
                    Content: "While Jesse paraded seven older sons before Samuel, David was left tending sheep in the field — considered too insignificant to even be called in. Yet the Lord told Samuel, \"man looks at the outward appearance, but the Lord looks at the heart\" (1 Sam. 16:7).",
                    Bullets: null),
                new(
                    Title: "The Anointing Precedes the Palace:",
                    Content: "David was anointed king in 1 Samuel 16, but did not sit on the throne until many years, many battles, and much wilderness wandering later in 2 Samuel 5. A brand new beginning is often declared long before it is fully realized.",
                    Bullets: null),
                new(
                    Title: "God Prepares in Obscurity Before He Promotes in Public",
                    Content: null,
                    Bullets: new List<string>
                    {
                        "The Lesson: The unseen years — tending sheep, fighting the lion and the bear, playing the harp for a troubled king — were not wasted; they were training for a throne David did not yet know he would occupy.",
                        "David's Example: The same courage and skill David learned defending sheep from predators later defended Israel from Goliath and the Philistines.",
                        "Application: Do not despise small, hidden responsibilities — God is often building in private what He intends to display in public."
                    })
            },
            PrefaceSection: new PrefaceSectionDto(
                MainHeading: "lessons we can learn from God who gave a brand new beginning to David in the field",
                Preamble: "The story of David's anointing (1 Samuel 16:1-13) is one of the clearest pictures of a brand new beginning in Scripture — a shepherd boy, overlooked by his own family, is called in from the field and anointed to become the next king of Israel while the reigning king, Saul, still sat on the throne. The anointing was immediate; the enthronement was not.",
                SubHeading: "Here are the key lessons we can learn from David's new beginning:"
            ),
            DetailedLessons: new List<DetailedLessonDto>
            {
                new(
                    Title: "A New Beginning Can Start Before Anyone Notices",
                    Bullets: new List<string>
                    {
                        "The Lesson: David's brothers didn't expect him to be called, and even Samuel initially looked past him. God's new beginning for your life may start quietly, without fanfare or recognition from those closest to you.",
                        "David's Example: \"There remains yet the youngest, and behold, he is keeping the sheep\" (1 Sam. 16:11) — an afterthought to his own family became God's first choice.",
                        "Application: Do not measure your calling by how visible or celebrated it currently is."
                    }),
                new(
                    Title: "Faithfulness in the Field Prepares You for the Throne",
                    Bullets: new List<string>
                    {
                        "The Lesson: The skills, character, and courage God will need from you in your new season are being formed right now, in whatever \"field\" you currently occupy.",
                        "David's Example: David told Saul he had killed both a lion and a bear while protecting his father's sheep — proof that his private faithfulness had already prepared him for public battle.",
                        "Application: Give your full diligence to your present responsibilities, however small; they may be the very training ground for what God is about to do."
                    }),
                new(
                    Title: "The Anointing Does Not Remove the Wilderness",
                    Bullets: new List<string>
                    {
                        "The Lesson: David was anointed king immediately, but then spent years being pursued by Saul, hiding in caves, and living as a fugitive before he ever wore a crown.",
                        "David's Example: Despite being God's chosen king, David refused to take Saul's life or force his way onto the throne, trusting God's timing instead.",
                        "Application: Receiving God's promise of a new beginning does not mean the difficult season ends immediately — remain faithful and refuse to force what only God can establish."
                    })
            }
        );

        return PastorPost.Create(
            title: "A Brand New Beginning – David",
            content: "The anointing of David in 1 Samuel 16 shows how God can call the overlooked, the youngest, and the unnoticed into a brand new beginning — even while the old order still stands.",
            category: PostCategory.Devotional,
            themeOfTheYearId: themeId,
            authorName: "Pastor",
            excerpt: "The anointing of David in 1 Samuel 16 shows how God can call the overlooked, the youngest, and the unnoticed into a brand new beginning — even while the old order still stands.",
            introHeading: "INTRODUCTION",
            introText: "The story of David's anointing (1 Samuel 16:1-13) provides a powerful picture of a brand new beginning that starts long before it is ever seen by others. While his own father did not think to call him in from the field, God had already chosen David to be the next king of Israel. David's journey from the sheep pasture to the throne teaches that God's new beginnings often start in obscurity and are proven through seasons of hidden faithfulness before they are ever publicly established.",
            structuredContentJson: StructuredContentDto.Serialize(structuredContent),
            closingText: "As we walk in this brand new beginning, may we, like David, remain faithful in the field while we wait for God to establish us on the throne He has already prepared.",
            bibleReference: "1 SAMUEL 16:1-13"
        );
    }

    // ============================================================
    // ESTHER — crowned queen in place of Vashti
    // ============================================================
    private static PastorPost BuildEstherPost(Guid themeId)
    {
        var structuredContent = new StructuredContentDto(
            FirstPoints: new List<FirstPointDto>
            {
                new(
                    Title: "A New Beginning Can Emerge From Someone Else's Ending:",
                    Content: "Queen Vashti's removal from the throne created the very opening through which Esther, an orphaned Jewish girl in exile, would rise to become queen of Persia (Esther 1-2). What looked like the king's crisis became God's setup.",
                    Bullets: null),
                new(
                    Title: "Hidden Identity, Divine Purpose:",
                    Content: "Esther kept her Jewish identity concealed at Mordecai's instruction for a season, yet God was positioning her all along \"for such a time as this\" (Esther 4:14) to save her people from destruction.",
                    Bullets: null),
                new(
                    Title: "God Positions Before He Reveals the Purpose",
                    Content: null,
                    Bullets: new List<string>
                    {
                        "The Lesson: Esther did not know, at the moment of her coronation, why God had placed her in the palace. The purpose was only revealed years later, when Haman's plot to destroy the Jews arose.",
                        "Esther's Example: Esther's beauty and favor secured her the crown, but it was her courage before the king — risking her life to intercede — that fulfilled God's actual purpose for her new beginning.",
                        "Application: A new beginning is rarely given for its own sake; ask God to reveal what He has positioned you for, and be ready to act when the moment comes."
                    })
            },
            PrefaceSection: new PrefaceSectionDto(
                MainHeading: "lessons we can learn from God who gave a brand new beginning to Esther in the palace",
                Preamble: "The book of Esther tells the story of a Jewish orphan girl, raised by her cousin Mordecai in exile, who is chosen from among the young women of the kingdom and crowned queen of Persia in place of the deposed Queen Vashti (Esther 2:1-18). Her new beginning was not merely a personal elevation — it became the very means by which an entire nation was saved.",
                SubHeading: "Here are the key lessons we can learn from Esther's new beginning:"
            ),
            DetailedLessons: new List<DetailedLessonDto>
            {
                new(
                    Title: "Your New Beginning May Be for More Than Just You",
                    Bullets: new List<string>
                    {
                        "The Lesson: Esther's rise to the throne was not simply a personal blessing; it existed to serve a purpose far bigger than herself — the deliverance of her entire people.",
                        "Esther's Example: Mordecai reminded her, \"who knows whether you have not attained royalty for such a time as this?\" (Esther 4:14), reframing her position as assignment, not just privilege.",
                        "Application: When God gives you a new beginning, ask what — and who — it might be for, beyond your own comfort or advancement."
                    }),
                new(
                    Title: "Favor Opens Doors, But Courage Walks Through Them",
                    Bullets: new List<string>
                    {
                        "The Lesson: Esther's beauty and favor got her into the palace, but her courage — approaching the king unsummoned, at risk of death — is what actually accomplished God's purpose.",
                        "Esther's Example: \"If I perish, I perish\" (Esther 4:16) — Esther chose obedience over safety once she understood what her position demanded of her.",
                        "Application: Do not mistake the favor that brought you into a new season for the whole assignment; be willing to act courageously when the moment for purpose arrives."
                    }),
                new(
                    Title: "What Looks Like Someone's Loss Can Be God's Setup for You",
                    Bullets: new List<string>
                    {
                        "The Lesson: Vashti's removal from the throne was, from a human standpoint, simply a political crisis in the Persian court — yet it created the exact vacancy God used to position Esther.",
                        "Esther's Example: Esther did not manipulate or scheme her way to the throne; she was chosen through a process already set in motion by circumstances beyond her control.",
                        "Application: Trust that God can turn situations that have nothing to do with you into the very doorway of your own new beginning — remain ready, not anxious."
                    })
            }
        );

        return PastorPost.Create(
            title: "A Brand New Beginning – Esther",
            content: "Esther's coronation in place of Queen Vashti shows how God can turn someone else's ending into your new beginning, positioning you long before the purpose is revealed.",
            category: PostCategory.Devotional,
            themeOfTheYearId: themeId,
            authorName: "Pastor",
            excerpt: "Esther's coronation in place of Queen Vashti shows how God can turn someone else's ending into your new beginning, positioning you long before the purpose is revealed.",
            introHeading: "INTRODUCTION",
            introText: "The story of Esther (Esther 2:1-18) shows a brand new beginning built on divine positioning rather than personal ambition. An orphaned Jewish girl in a foreign land is crowned queen of Persia in place of the deposed Vashti — not knowing that her elevation was preparing her to save her entire nation from destruction. Esther's journey teaches that God often places us in new seasons long before He reveals why.",
            structuredContentJson: StructuredContentDto.Serialize(structuredContent),
            closingText: "May we, like Esther, recognize that our new beginning may exist for such a time as this — and may we have the courage to act when that time comes.",
            bibleReference: "ESTHER 2:1-18"
        );
    }

    // ============================================================
    // RUTH — grafted into the covenant with Israel
    // ============================================================
    private static PastorPost BuildRuthPost(Guid themeId)
    {
        var structuredContent = new StructuredContentDto(
            FirstPoints: new List<FirstPointDto>
            {
                new(
                    Title: "A New Beginning Can Require Leaving the Familiar:",
                    Content: "Ruth, a Moabite widow with no blood claim to Israel, chose to leave her homeland, her gods, and her people to follow Naomi, declaring \"your people shall be my people, and your God my God\" (Ruth 1:16). Her new beginning began with a costly choice.",
                    Bullets: null),
                new(
                    Title: "Loyalty in the Field Precedes Position in the Family:",
                    Content: "Before Ruth became part of the covenant lineage through marriage to Boaz, she first proved her character gleaning in his fields — faithful, hardworking, and respectful, unseen by anyone of consequence except Boaz's watching eyes.",
                    Bullets: null),
                new(
                    Title: "God Grafts Outsiders Into His Covenant Purpose",
                    Content: null,
                    Bullets: new List<string>
                    {
                        "The Lesson: Ruth had no natural right to belong to Israel's covenant community — she was a foreigner, a Moabite, from a nation historically at odds with God's people. Yet God wove her into the very lineage of King David, and ultimately, of Christ.",
                        "Ruth's Example: \"So Boaz took Ruth, and she became his wife\" (Ruth 4:13) — a Moabite widow became an ancestress of the Messiah.",
                        "Application: No background, history, or outsider status disqualifies you from God's covenant purpose when you choose to align yourself with His people."
                    })
            },
            PrefaceSection: new PrefaceSectionDto(
                MainHeading: "lessons we can learn from God who gave a brand new beginning to Ruth in Israel",
                Preamble: "The book of Ruth tells the story of a Moabite widow who, through loyalty, humility, and God's providence, was grafted into the covenant community of Israel — ultimately becoming part of the lineage of King David and, by extension, of Jesus Christ. Ruth's new beginning was built not on birthright, but on faithful choice and divine grace.",
                SubHeading: "Here are the key lessons we can learn from Ruth's new beginning:"
            ),
            DetailedLessons: new List<DetailedLessonDto>
            {
                new(
                    Title: "New Beginnings Often Start With a Costly Decision",
                    Bullets: new List<string>
                    {
                        "The Lesson: Ruth's new beginning did not begin with blessing, but with loss — the death of her husband — and a difficult choice to leave everything familiar behind.",
                        "Ruth's Example: Unlike Orpah, who returned to her people, Ruth clung to Naomi and to Naomi's God, choosing an uncertain future in a foreign land over the security of home.",
                        "Application: Be willing to release what is familiar, even when it is costly, if it stands between you and the new beginning God is calling you into."
                    }),
                new(
                    Title: "Faithfulness in Obscurity Is Seen by God, Even When Unseen by Man",
                    Bullets: new List<string>
                    {
                        "The Lesson: Ruth's diligence gleaning leftover grain in Boaz's field was humble, unglamorous work — yet it was there that her character was noticed and her provision began.",
                        "Ruth's Example: Boaz testified, \"It has been fully reported to me all that you have done for your mother-in-law\" (Ruth 2:11) — her reputation for faithfulness had preceded her.",
                        "Application: Do not despise humble beginnings; faithfulness in small, unseen places often becomes the foundation for God's larger plan."
                    }),
                new(
                    Title: "Being Grafted In Is an Act of Grace, Not Merit",
                    Bullets: new List<string>
                    {
                        "The Lesson: Ruth could never have earned her place in Israel's covenant lineage by birth — her inclusion was entirely a matter of grace, extended through Boaz as kinsman-redeemer.",
                        "Ruth's Example: Boaz's act of redemption (Ruth 4) formally brought Ruth, an outsider, fully into the covenant family — she was not just tolerated, but established.",
                        "Application: Receive your new beginning as an act of grace rather than something to be earned, and extend that same grace to others who are being grafted into what God is building."
                    })
            }
        );

        return PastorPost.Create(
            title: "A Brand New Beginning – Ruth",
            content: "Ruth's integration into the covenant community of Israel shows how God grafts in the outsider, turning a costly choice and humble faithfulness into a brand new beginning within His covenant purpose.",
            category: PostCategory.Devotional,
            themeOfTheYearId: themeId,
            authorName: "Pastor",
            excerpt: "Ruth's integration into the covenant community of Israel shows how God grafts in the outsider, turning a costly choice and humble faithfulness into a brand new beginning within His covenant purpose.",
            introHeading: "INTRODUCTION",
            introText: "The book of Ruth presents a brand new beginning shaped entirely by loyalty and grace. A Moabite widow with no natural claim to Israel's covenant chooses to leave her homeland and follow her mother-in-law's God, and through faithful humility and divine providence, is grafted into the very lineage of King David and ultimately of Christ. Ruth's story teaches that God's new beginnings are available to anyone willing to choose Him, regardless of background.",
            structuredContentJson: StructuredContentDto.Serialize(structuredContent),
            closingText: "As we embrace this brand new beginning, may we, like Ruth, choose faithfulness over familiarity and trust God to graft us fully into what He is building.",
            bibleReference: "RUTH 1-4"
        );
    }
}