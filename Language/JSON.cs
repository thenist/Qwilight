namespace Language
{
    public struct JSON
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
            public MSG message = new();

            public N2MT()
            {
            }

            public struct MSG
            {
                public Data result = new();

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