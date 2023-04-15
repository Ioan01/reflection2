namespace reflection2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var file = File.Open("script.txt",FileMode.Open);

            StreamReader aReader = new StreamReader(file);

            var str = aReader.ReadToEnd();


            Parser.InterpretScript(str);
        }
    }
}