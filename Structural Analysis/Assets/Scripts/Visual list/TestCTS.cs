using System.IO;
using System.Reflection;

using UnityEngine;

public class TestCTS : MonoBehaviour
{
    private const int COUNT_TEST = 3;

    private void Start()
    {
        DirectoryInfo directory = new(Assembly.GetExecutingAssembly().Location.Replace("\\Library\\ScriptAssemblies\\Assembly-CSharp.dll", ""));
        if (directory.GetFiles().Length > 0)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name.EndsWith(".txt"))
                {
                    try
                    {
                        ReadMatrix(file.Name.Replace(".txt", ""));
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
    private void ReadMatrix(string path)
    {
        string text;
        using StreamReader reader = new($"{path}.txt");
        {
            int count = -1;
            int y = 0;
            int[,] matrix = null;
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split('\t');
                if (count == -1)
                {
                    count = line.Length;
                    matrix = new int[count, count];
                }

                for (int x = 0; x < count; x++)
                    matrix[x, y] = line[x].ToInt();
                y++;
            }

            text = GetOrderCalculation(matrix.Transposition());
        }

        FileStream f = new FileStream($"{path}.xls", FileMode.Create);
        f.Close();
        using StreamWriter writer = new($"{path}.xls");
        writer.Write(text);
    }
    private string GetOrderCalculation(int[,] matrix) => new StructuralAnalysisCTS().StructuralAnalysisChemicalTechnologicalSystems(matrix);

}
