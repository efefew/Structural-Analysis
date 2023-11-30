using System.IO;
using System.Reflection;

using UnityEngine;

public class TestCTS : MonoBehaviour
{
    private const int COUNT_TEST = 3;

    private void Start()
    {
        Debug.Log(Assembly.GetExecutingAssembly().Location.Replace("\\Library\\ScriptAssemblies\\Assembly-CSharp.dll", ""));
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
            int count = reader.ReadLine().ToInt();
            int y = 0;
            int[,] matrix = new int[count, count];
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split('\t');
                for (int x = 0; x < count; x++)
                    matrix[x, y] = line[x].ToInt();
                y++;
            }

            text = GetOrderCalculation(matrix.Transposition());
        }

        _ = new FileStream($"{path}.xls", FileMode.Create);
        using StreamWriter writer = new($"{path}.xls");
        writer.Write(text);
    }
    private string GetOrderCalculation(int[,] matrix) => new StructuralAnalysisCTS().StructuralAnalysisChemicalTechnologicalSystems(matrix);

}
