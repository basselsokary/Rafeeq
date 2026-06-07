export const SITE_TYPES = [
  'other','historical','museum','religious','archaeological','cultural',
  'natural','entertainment','monument','palace','temple','park','zoo',
  'aquarium','botanicalGarden','castle','fortress','gallery','library',
  'theater','stadium','beach','mountain','lake','river','waterfall',
  'cave','volcano','desert','island','gate','therapeutic','divingSite',
  'marina','walkway',
];

export const SITE_STATUSES = [
  'active',
  'underMaintenance',
  'temporarilyClosed',
  'permanentlyClosed',
];

export const LANGUAGE_CODES = [
  'arabic','english','german','russian','french','spanish',
];

export const FACILITY_TYPES = [
  'restrooms','parking','cafeteriaOrRestaurant','giftShop','informationCenter',
  'firstAid','wiFi','atm','prayerRoom','wheelchairAccess','audioGuide',
  'lockers','babyChanging','petFriendly','photographyAllowed','guidedTours',
  'bicycleRental','strollerAccess','signLanguageTours','elevator','galleries',
  'exhibition','theaters','gardens','library','readingRooms','boardRides',
  'localMarkets','natureTrails',
];

export const ATTRACTION_TYPES = [
  'other','pyramid','temple','tomb','statue','monument','mosque','church',
  'synagogue','museumHall','exhibition','palace','fortress','gate','obelisk',
  'column','ruins','garden','courtyard','cave','rockCutStructure',
  'archaeologicalStructure','historicBuilding','viewingPoint','culturalExhibit',
];

export const ARTIFACT_TYPES = [
  'other','statue','mask','sphinx','stele','bust','painting','manuscript',
  'jewelry','coin','vessel','weapon','tool','textile','furniture',
  'musicalInstrument','ceramic','pottery','glassware','bone','ivory',
  'shell','metalwork','shrine',
];

export const HISTORICAL_PERIODS = [
  'unknown','predynastic','earlyDynastic','oldKingdom','firstIntermediatePeriod',
  'middleKingdom','secondIntermediatePeriod','newKingdom','thirdIntermediatePeriod',
  'latePeriod','ptolemaic','roman','byzantine','rashidun','umayyad','abbasid',
  'tulunid','ikhshidid','fatimid','ayyubid','bahriMamluk','burjiMamluk',
  'ottoman','muhammadAliDynasty',
];

export const SPONSOR_TYPES = ['restaurant','hotel','shop','service','tour','transportation'];

export const SPONSOR_TIERS = ['bronze','silver','gold','platinum'];

export const WEEKDAYS = [
  'saturday','sunday','monday','tuesday','wednesday','thursday','friday',
];

export const STATUS_COLORS = {
  active:             'green',
  underMaintenance:   'yellow',
  temporarilyClosed:  'red',
  permanentlyClosed:  'gray',
};

export const STATUS_LABELS = {
  active:             'Active',
  underMaintenance:   'Under Maintenance',
  temporarilyClosed:  'Temporarily Closed',
  permanentlyClosed:  'Permanently Closed',
};

export const ROLES = {
  superAdmin: 'SuperAdmin',
  admin:     'Admin',
  moderator: 'Moderator',
  tourist: 'Tourist',
};

export const formatEnum = (val) =>
  val
    ? val.replace(/([A-Z])/g, ' $1').replace(/^./, (s) => s.toUpperCase())
    : '—';
