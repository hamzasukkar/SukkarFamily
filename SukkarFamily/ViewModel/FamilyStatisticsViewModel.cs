using System;

namespace SukkarFamily.ViewModel
{
    public class FamilyStatisticsViewModel
    {
        public int TotalMembers { get; set; }
        public int TotalGenerations { get; set; }
        public int MaleMembers { get; set; }
        public int FemaleMembers { get; set; }
        public string OldestMemberName { get; set; }
        public string YoungestMemberName { get; set; }
        public int LivingMembers { get; set; }
        public DateTime? EarliestBirthYear { get; set; }
        public DateTime? LatestBirthYear { get; set; }
        public string MostCommonLocation { get; set; }
        public int UniqueLocations { get; set; }
    }
}