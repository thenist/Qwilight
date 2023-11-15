namespace Language
{
    public static class JSON
    {
        public struct Client
        {
            public string nhnID = string.Empty;
            public string nhnPw = string.Empty;

            public Client()
            {
            }
        }

        public struct N2MT
        {
            public MSG message = new MSG();

            public N2MT()
            {
            }

            public struct MSG
            {
                public Data result = new Data();

                public MSG()
                {
                }

                public struct Data
                {
                    public string translatedText = string.Empty;

                    public Data()
                    {
                    }
                }
            }
        }
    }
}