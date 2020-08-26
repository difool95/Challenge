using System.Collections.Generic;
using UnityEngine;

class DataManager : MonoBehaviour
{
    public List<Animal> animals;

    public List<Animal> GetRandomAnimals()
    {
        List<Animal> animalsToDisplay = new List<Animal>();
        int random = Random.Range(0, animals.Count);
        animalsToDisplay.Add(animals[random]);
        animals.RemoveAt(random);
        
        int random2 = Random.Range(0, animals.Count);
        animalsToDisplay.Add(animals[random2]);
        animals.RemoveAt(random2);
        
        int random3 = Random.Range(0, animals.Count);
        animalsToDisplay.Add(animals[random3]);
        animals.RemoveAt(random3);
        
        return animalsToDisplay;
    }

   
}
