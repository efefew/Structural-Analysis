using System.IO;

using UnityEngine;

public class TestCTS : MonoBehaviour
{
    private const int COUNT_TEST = 3;

    private void Start()
    {
        for (int idTest = 1; idTest <= COUNT_TEST; idTest++)
            ReadMatrix($"{idTest}");
    }
    private void ReadMatrix(string path)
    {
        using StreamReader reader = new($"{path}.txt");
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

        GetOrderCalculation(matrix.Transposition());
    }
    private void GetOrderCalculation(int[,] matrix) => Debug.Log(new StructuralAnalysisCTS().StructuralAnalysisChemicalTechnologicalSystems(matrix));

}
