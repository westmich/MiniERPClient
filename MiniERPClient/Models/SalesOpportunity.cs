namespace MiniERPClient.Models
{
    public enum SalesOpportunityStage
    {
        Lead,
        Qualified,
        Proposal,
        Negotiation,
        ClosedWon,
        ClosedLost
    }

    public class SalesOpportunity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedValue { get; set; }
        public int ProbabilityPercent { get; set; } = 50;
        public SalesOpportunityStage Stage { get; set; } = SalesOpportunityStage.Lead;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ExpectedCloseDate { get; set; }
        public DateTime? ActualCloseDate { get; set; }
        public string SalesRepresentative { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation Property (for demo purposes, we'll populate this manually)
        public Customer? Customer { get; set; }

        // Computed Properties
        public decimal WeightedValue => EstimatedValue * (ProbabilityPercent / 100m);
        public string StageBadge => Stage.ToString();
        public int DaysOpen => (DateTime.Now - CreatedDate).Days;
        public bool IsOverdue => ExpectedCloseDate.HasValue && DateTime.Now > ExpectedCloseDate.Value && Stage != SalesOpportunityStage.ClosedWon && Stage != SalesOpportunityStage.ClosedLost;
    }
}