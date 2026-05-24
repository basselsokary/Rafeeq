namespace Domain.Enums;

/// <summary>
/// Represents the major historical periods of Egypt.
/// Values are explicitly assigned to ensure stability (do not reorder).
/// </summary>
public enum HistoricalPeriod
{
    Unknown = 0,
    /// <summary>
    /// c. 6000 – 3150 BC
    /// Before unification; early settlements and cultural development.
    /// </summary>
    Predynastic = 1,

    /// <summary>
    /// c. 3150 – 2686 BC
    /// Unification of Upper and Lower Egypt; 1st–2nd Dynasties.
    /// </summary>
    EarlyDynastic = 2,

    /// <summary>
    /// c. 2686 – 2181 BC
    /// Pyramid age; strong centralized state (3rd–6th Dynasties).
    /// </summary>
    OldKingdom = 3,

    /// <summary>
    /// c. 2181 – 2055 BC
    /// Political fragmentation and regional rule.
    /// </summary>
    FirstIntermediatePeriod = 4,

    /// <summary>
    /// c. 2055 – 1650 BC
    /// Reunification and cultural renaissance (11th–12th Dynasties).
    /// </summary>
    MiddleKingdom = 5,

    /// <summary>
    /// c. 1650 – 1550 BC
    /// Hyksos rule and decline of central authority.
    /// </summary>
    SecondIntermediatePeriod = 6,

    /// <summary>
    /// c. 1550 – 1069 BC
    /// Imperial Egypt; expansion and wealth (18th–20th Dynasties).
    /// </summary>
    NewKingdom = 7,

    /// <summary>
    /// c. 1069 – 664 BC
    /// Division and foreign influence (21st–25th Dynasties).
    /// </summary>
    ThirdIntermediatePeriod = 8,

    /// <summary>
    /// c. 664 – 332 BC
    /// Final phase of native rule before conquest by Alexander the Great.
    /// </summary>
    LatePeriod = 9,

    /// <summary>
    /// 332 – 30 BC
    /// Greek rule following Alexander; Ptolemaic Dynasty.
    /// </summary>
    Ptolemaic = 10,

    /// <summary>
    /// 30 BC – 395 AD
    /// Egypt as a province of the Roman Empire.
    /// </summary>
    Roman = 11,

    /// <summary>
    /// 395 – 641 AD
    /// Eastern Roman (Byzantine) rule.
    /// </summary>
    Byzantine = 12,

    /// <summary>
    /// 641 – 661 AD
    /// Early Islamic rule under the Rashidun Caliphate.
    /// </summary>
    Rashidun = 13,

    /// <summary>
    /// 661 – 750 AD
    /// Umayyad Caliphate governance.
    /// </summary>
    Umayyad = 14,

    /// <summary>
    /// 750 – 868 AD
    /// Abbasid Caliphate control.
    /// </summary>
    Abbasid = 15,

    /// <summary>
    /// 868 – 905 AD
    /// Semi-independent Tulunid Dynasty.
    /// </summary>
    Tulunid = 16,

    /// <summary>
    /// 935 – 969 AD
    /// Ikhshidid Dynasty rule.
    /// </summary>
    Ikhshidid = 17,

    /// <summary>
    /// 969 – 1171 AD
    /// Fatimid Caliphate; Cairo founded.
    /// </summary>
    Fatimid = 18,

    /// <summary>
    /// 1171 – 1250 AD
    /// Ayyubid Dynasty; founded by صلاح الدين.
    /// </summary>
    Ayyubid = 19,

    /// <summary>
    /// 1250 – 1382 AD
    /// Bahri Mamluk Sultanate.
    /// </summary>
    BahriMamluk = 20,

    /// <summary>
    /// 1382 – 1517 AD
    /// Burji (Circassian) Mamluk Sultanate.
    /// </summary>
    BurjiMamluk = 21,

    /// <summary>
    /// 1517 – 1805 AD
    /// Ottoman provincial rule.
    /// </summary>
    Ottoman = 22,

    /// <summary>
    /// 1805 – 1952 AD
    /// Muhammad Ali Dynasty; modernization of Egypt.
    /// </summary>
    MuhammadAliDynasty = 23,
    
    /// <summary>
    /// 250 – 450 AD
    /// Late Antique/Early Coptic period.
    /// </summary>
    LateAntiqueEarlyCoptic = 24,
    /// <summary>
    /// 1952 – 1956 AD
    /// Contemporary period.
    /// </summary>
    Contemporary = 25,
    /// <summary>
    /// 1956 – 1971 AD
    /// Modern Era.
    /// </summary>
    ModernEra = 26,
    /// <summary>
    /// 1867 – 1956 AD
    /// Khedivial era.
    /// </summary>
    KhedivialEra = 27,
    Islamic = 28, // General category for attractions spanning multiple Islamic periods.
}