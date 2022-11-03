namespace twtTest.Data
{
    public class InsuranceClaims
    {
        private List<InsuranceClaim> claims = new();

        public List<InsuranceClaim> ValidInsuranceClaims
        {
            get { return claims; }
            set { claims = value; }
        }
            
    }
}
