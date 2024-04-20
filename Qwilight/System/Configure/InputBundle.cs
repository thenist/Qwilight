namespace Qwilight
{
    public sealed class InputBundle<T> where T : new()
    {
        public T[][][] Inputs { get; set; }

        public T[] StandardInputs { get; set; }

        public InputBundle()
        {
            SetInputs();
            SetStandardInputs();
        }

        public void SetInputs()
        {
            Inputs = new T[17][][];
            for (var i = Inputs.Length - 1; i >= 0; --i)
            {
                Inputs[i] = new T[Component.InputCounts[i] + 1][];
                for (var j = Inputs[i].Length - 1; j >= 0; --j)
                {
                    Inputs[i][j] = new T[5];
                    for (var m = Inputs[i][j].Length - 1; m >= 0; --m)
                    {
                        Inputs[i][j][m] = new T();
                    }
                }
            }
        }

        public void SetStandardInputs()
        {
            StandardInputs = new T[typeof(T) != typeof(DefaultInput) ? 18 : 12];
            for (var i = StandardInputs.Length - 1; i >= 0; --i)
            {
                StandardInputs[i] = new T();
            }
        }
    }
}
