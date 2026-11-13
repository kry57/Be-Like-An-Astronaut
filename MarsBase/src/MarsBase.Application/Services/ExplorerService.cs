using System.Text.Json;
using MarsBase.Application.Abstractions;
using MarsBase.Application.Common;
using MarsBase.Application.Contracts;
using MarsBase.Domain.Entities;

namespace MarsBase.Application.Services;

public sealed class ExplorerService(IExplorerSessionRepository repository, IClock clock) : IExplorerService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly IReadOnlyDictionary<string, DecisionData> Decisions =
        new Dictionary<string, DecisionData>
    {
        ["1_1"] = new(94, 88, 91, 76, 98, 92, 250, 3, "البطاريات الاحتياطية", "البطاريات الاحتياطية تخزن الطاقة الشمسية لاستخدامها عندما تحجب العواصف الترابية نور الشمس على المريخ."),
        ["1_2"] = new(94, 88, 94, 76, 98, 94, 300, 3, "مصدر طاقة نووي صغير (NASA Kilopower)", "طورت وكالة ناسا مفاعل (Kilopower) الصغير ليمنح رواد المريخ من 1 إلى 10 كيلوواط من الكهرباء المستمرة دون الحاجة للشمس!"),
        ["1_3"] = new(90, 88, 93, 76, 98, 91, 180, 2, "وضع توفير الطاقة", "تقليل التهوية يوفر الكهرباء مؤقتاً، لكن جهاز (MOXIE) الذي اختبرته ناسا يحتاج للطاقة لتحويل ثاني أكسيد الكربون إلى أكسجين."),
        ["2_1"] = new(94, 82, 91, 80, 98, 92, 220, 2, "غسل التربة بالمياه", "المياه تغسل أملاح البيركلورات الضارة من تربة المريخ، لكن الماء الجليدي على المريخ ثروة ثمينة يجب ترشيدها!"),
        ["2_2"] = new(96, 86, 88, 82, 98, 94, 300, 3, "الزراعة بدون تربة (NASA Veggie)", "الزراعة المائية بدون تربة تحمي النباتات من سموم تربة المريخ وتنتج غذاءً طازجاً وأكسجيناً إضافياً للرواد!"),
        ["2_3"] = new(95, 88, 91, 78, 98, 93, 260, 3, "الميكروبات المنقية", "البكتيريا والكائنات الدقيقة النافعة تفكك المواد الكيميائية الضارة في التربة بطريقة طبيعية صديقة للبيئة."),
        ["3_1"] = new(94, 86, 91, 76, 98, 92, 210, 2, "ترقيع سريع للأنابيب", "الرقعة السريعة توقف تسريب سائل التبريد مؤقتاً، لكن ضغط الأنابيب الحرارية يتطلب فحصاً مستمراً."),
        ["3_2"] = new(95, 90, 91, 76, 99, 95, 300, 3, "تركيب أنابيب تيتانيوم جديدة", "تستخدم ناسا أنابيب تبريد متطورة من التيتانيوم لنقل الحرارة بأمان؛ استبدال الأنابيب يحمي القاعدة بشكل دائم!"),
        ["3_3"] = new(92, 83, 95, 76, 96, 90, 170, 2, "توفير الطاقة والإصلاح لاحقاً", "تأجيل صيانة أنابيب التبريد يوفر الطاقة في البداية لكنه يعرض أجهزة دعم الحياة لارتفاع الحرارة."),
        ["4_1"] = new(94, 88, 86, 76, 100, 95, 300, 3, "الدرع المغناطيسي الأقصى", "بما أن الغلاف الجوي للمريخ رقيق جداً، فإن تفعيل الدرع المغناطيسي يحمي الرواد من الجسيمات الشمسية المشحونة!"),
        ["4_2"] = new(93, 88, 91, 76, 99, 94, 270, 3, "مخبأ الحماية الأرضي", "الاحتماء تحت سطح المريخ مع ترك عربات الروفر والروبوتات تعمل بالخارج يحمي صحة الفريق 100%."),
        ["4_3"] = new(94, 88, 95, 76, 95, 91, 200, 2, "عاكسات القباب الذكية", "استغلال العاصفة لتوليد الكهرباء يرفع مخزون الطاقة، لكن تقوية دروع الإشعاع أهم لسلامة الطاقم."),
        ["5_1"] = new(94, 88, 89, 79, 99, 94, 280, 3, "شباك الصد الفولاذية", "الغلاف الجوي الرقيق للمريخ يسمح بمرور الشهب الصغيرة؛ إغلاق الأغطية المدرعة يحمي زجاج القباب الزراعية."),
        ["5_2"] = new(95, 88, 87, 80, 100, 95, 300, 3, "منظومة ليزر الدفاع (MDS Laser)", "تماماً كما في لعبة محاكاة الفضاء العالمية (Surviving Mars)، يقوم ليزر الدفاع الجوي بتفتيت الشهب قبل وصولها للقاعدة!"),
        ["5_3"] = new(94, 86, 92, 76, 98, 92, 210, 2, "تعديل زاوية الدروع", "توجيه زاوية الدروع يقلل أثر الشظايا الصغيرة بأقل استهلاك للطاقة."),
        ["6_1"] = new(94, 88, 89, 76, 98, 94, 290, 3, "قمر التقوية المداري (MRO)", "تستخدم ناسا مدارات فضائية حول المريخ مثل (Mars Reconnaissance Orbiter) لنقل إشارات عربات الروفر إلى شبكة الفضاء العميق (DSN)!"),
        ["6_2"] = new(95, 89, 88, 78, 99, 96, 350, 3, "توجيه هوائي الليزر الضوئي", "الاتصالات الضوئية بالليزر تنقل الصور والبيانات العلمية من المريخ إلى كوكب الأرض بسرعة الضوء!"),
        ["6_3"] = new(94, 88, 91, 76, 98, 93, 230, 2, "إعادة ضبط الترددات يدوياً", "تنظيف الهوائيات من غبار المريخ وإعادة توجيهها يضمن استمرار الاتصال مع مركز القيادة على الأرض."),
    };

    public async Task<Result<ExplorerStateDto>> StartAsync(StartExplorerRequest request, CancellationToken ct)
    {
        var mode = Normalize(request.PlayMode, ["solo", "duo", "squad"], "solo");
        var difficulty = Normalize(request.Difficulty, ["easy", "medium", "hard"], "easy");
        var hero = Normalize(request.Hero, ["adam", "layla", "both"], "adam");
        var expectedCount = mode switch { "duo" => 2, "squad" => 4, _ => 1 };
        var supplied = request.Players?.Take(expectedCount).ToList() ?? [];
        var defaults = new[] {
            new ExplorerPlayerRequest(request.PlayerName?.Trim() is { Length: > 0 } n ? n : "Space Explorer", 10, "Mission Leader"),
            new ExplorerPlayerRequest("Sara", 9, "Energy Engineer"),
            new ExplorerPlayerRequest("Youssef", 11, "Botanist"),
            new ExplorerPlayerRequest("Layla", 8, "Communications")
        };
        var players = Enumerable.Range(0, expectedCount).Select(i =>
        {
            var p = i < supplied.Count ? supplied[i] : defaults[i];
            return new ExplorerPlayerDto(string.IsNullOrWhiteSpace(p.Name) ? defaults[i].Name : p.Name.Trim(),
                Math.Clamp(p.Age, 4, 99), p.Role ?? defaults[i].Role);
        }).ToList();

        var now = clock.UtcNow;
        var session = new ExplorerSession
        {
            Id = Guid.CreateVersion7(), PlayerName = players[0].Name, Hero = hero, PlayMode = mode,
            Difficulty = difficulty, CurrentScreenId = "1", CurrentLevel = 1, ActivePlayerIndex = 0,
            TotalXp = 0, O2 = 94, Water = 88, Solar = 91, Rations = 76, Shield = 98, Health = 92,
            StarsJson = JsonSerializer.Serialize(new Dictionary<string, int> { ["1"]=0, ["2"]=0, ["3"]=0, ["4"]=0, ["5"]=0, ["6"]=0 }),
            PlayersJson = JsonSerializer.Serialize(players, JsonOptions), HistoryJson = "[]",
            CreatedAtUtc = now, UpdatedAtUtc = now
        };
        await repository.AddAsync(session, ct);
        await repository.SaveChangesAsync(ct);
        return Result<ExplorerStateDto>.Success(ToDto(session));
    }

    public async Task<Result<ExplorerStateDto>> GetAsync(Guid id, CancellationToken ct)
    {
        var session = await repository.GetAsync(id, ct);
        return session is null ? GameErrors.SessionNotFound : Result<ExplorerStateDto>.Success(ToDto(session));
    }

    public async Task<Result<ExplorerStateDto>> RecordDecisionAsync(Guid id, ExplorerDecisionRequest request, CancellationToken ct)
    {
        var session = await repository.GetAsync(id, ct);
        if (session is null) return GameErrors.SessionNotFound;
        if (session.Status != "InProgress") return GameErrors.SessionFinished;
        var key = $"{request.Level}_{request.Option}";
        if (!Decisions.TryGetValue(key, out var decision)) return GameErrors.InvalidOption;

        var multiplier = session.Difficulty switch { "medium" => 1.25, "hard" => 1.5, _ => 1.0 };
        var gainedXp = (int)Math.Round(decision.Xp * multiplier);
        session.O2 = decision.O2; session.Water = decision.Water; session.Solar = decision.Solar;
        session.Rations = decision.Rations; session.Shield = decision.Shield; session.Health = decision.Health;
        session.TotalXp += gainedXp;
        var stars = ReadStars(session);
        stars[request.Level.ToString()] = Math.Max(stars[request.Level.ToString()], decision.Stars);
        session.StarsJson = JsonSerializer.Serialize(stars);
        var history = JsonSerializer.Deserialize<List<ExplorerDecisionDto>>(session.HistoryJson, JsonOptions) ?? [];
        history.Add(new ExplorerDecisionDto(request.Level, request.Option, decision.Title, decision.Lesson, gainedXp, decision.Stars));
        session.HistoryJson = JsonSerializer.Serialize(history, JsonOptions);
        session.CurrentLevel = request.Level;
        session.UpdatedAtUtc = clock.UtcNow;
        await repository.SaveChangesAsync(ct);
        return Result<ExplorerStateDto>.Success(ToDto(session));
    }

    public async Task<Result<ExplorerStateDto>> UpdateScreenAsync(Guid id, ExplorerScreenRequest request, CancellationToken ct)
    {
        var session = await repository.GetAsync(id, ct);
        if (session is null) return GameErrors.SessionNotFound;
        session.CurrentScreenId = request.ScreenId;
        session.CurrentLevel = request.Level;
        if (request.AdvanceTurn)
        {
            var players = ReadPlayers(session);
            session.ActivePlayerIndex = (session.ActivePlayerIndex + 1) % players.Count;
        }
        session.UpdatedAtUtc = clock.UtcNow;
        await repository.SaveChangesAsync(ct);
        return Result<ExplorerStateDto>.Success(ToDto(session));
    }

    public async Task<Result<ExplorerStateDto>> RestartAsync(Guid id, CancellationToken ct)
    {
        var previous = await repository.GetAsync(id, ct);
        if (previous is null) return GameErrors.SessionNotFound;
        var request = new StartExplorerRequest
        {
            PlayerName = previous.PlayerName, Hero = previous.Hero, PlayMode = previous.PlayMode,
            Difficulty = previous.Difficulty, Players = ReadPlayers(previous)
                .Select(p => new ExplorerPlayerRequest(p.Name, p.Age, p.Role)).ToList()
        };
        return await StartAsync(request, ct);
    }

    private static ExplorerStateDto ToDto(ExplorerSession s)
    {
        var players = ReadPlayers(s);
        var index = s.ActivePlayerIndex % players.Count;
        var stars = ReadStars(s);
        return new ExplorerStateDto(s.Id, s.PlayerName, s.Hero, s.PlayMode, s.Difficulty, s.Status,
            s.CurrentScreenId, s.CurrentLevel, index, players[index], players, s.TotalXp,
            stars.Values.Sum(), stars, new ExplorerTelemetryDto(s.O2, s.Water, s.Solar, s.Rations, s.Shield, s.Health),
            JsonSerializer.Deserialize<List<ExplorerDecisionDto>>(s.HistoryJson, JsonOptions) ?? []);
    }

    private static List<ExplorerPlayerDto> ReadPlayers(ExplorerSession s)
        => JsonSerializer.Deserialize<List<ExplorerPlayerDto>>(s.PlayersJson, JsonOptions) ?? [new("Space Explorer", 10, "Mission Leader")];

    private static Dictionary<string, int> ReadStars(ExplorerSession s)
        => JsonSerializer.Deserialize<Dictionary<string, int>>(s.StarsJson) ?? new();

    private static string Normalize(string? value, string[] allowed, string fallback)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return normalized is not null && allowed.Contains(normalized) ? normalized : fallback;
    }

    private sealed record DecisionData(int O2, int Water, int Solar, int Rations, int Shield, int Health,
        int Xp, int Stars, string Title, string Lesson);
}
