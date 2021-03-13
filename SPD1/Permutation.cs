using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<List<int>> GetAllPermutations(List<int> listOfElements,int count)
        {
            int[] current = new int[count];

            bool[] selection = new bool[count];

            Permutate(listOfElements, count, selection, current, 0);

            return listOfPermutations;
        }

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
    }
}
