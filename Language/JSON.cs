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
            public Message message = new Message();

            public N2MT()
            {
            }

            public struct Message
            {
                public Result result = new Result();

                public Message()
                {
                }

                public struct Result
                {
                    public string translatedText = string.Empty;

                    public Result()
                    {
                    }
                }
            }
        }
    }
}