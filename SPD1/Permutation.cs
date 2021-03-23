using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace SPD1
{
    public class Permutation
    {
        List<List<int>> listOfPermutations;
        public Permutation()
        {
            listOfPermutations = new List<List<int>>();
        }
        /// <summary>
        /// Generuje wszystkie permutacje zestawu danych podanych w argumencie
        /// zwraca listę wszytskich permutacji
        /// </summary>
        public List<List<int>> GetAllPermutations(List<int> listOfElements,int count)
        {
            int[] current = new int[count];

            bool[] selection = new bool[count];

            Permutate(listOfElements, count, selection, current, 0);

            return listOfPermutations;
        }

        /// <summary>
        /// Wykonuje właściwą permutację zestawu danych - listy elementów typu int
        /// Do funkcji podawane są dwie tablice:
        /// isSelected - tablica bool - na początku podaje się tablice wypełnioną false
        /// newPermutation - tablica permutacji - na początku pusta, stopniowo w trakcie pracy 
        /// funkcji uzupełniana o kolejne elementy
        /// Argument step - oznacza miejsce od którego rusza permutacja - w przypadku wykonywania pełnej permutacji step =0
        /// </summary>
        public void Permutate(List<int> listOfElements,int count, bool[] isSelected, int[] newPermutation, int step)
        {
            if(step == count)
            {
                listOfPermutations.Add(newPermutation.ToList());
            }
            else
            {
                for(int i =0; i< count; i++)
                {
                    if (!isSelected[i])
                    {
                        isSelected[i] = true;
                        newPermutation[step] = listOfElements[i];
                        Permutate(listOfElements, count, isSelected, newPermutation, step + 1);
                        isSelected[i] = false;
                    }
                }
            }
        }

        public static void RotateRight(IList sequence, int count)
        {
            object tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        public static IEnumerable<IList> Permutate(IList sequence, int count)
        {
            if (count == 1) yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in Permutate(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }
    }
}
