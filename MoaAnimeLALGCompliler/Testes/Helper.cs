using System.IO;

namespace Testes
{
    public class Helper
    {
      
      
        public static string GetExemploCorretoLalg()
        {
          return File.ReadAllText("EntradaCorreta.txt");
        }
    }
}