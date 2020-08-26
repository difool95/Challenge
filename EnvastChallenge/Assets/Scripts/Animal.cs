using System;

[Serializable]
struct Animal
{
    public string imgInResources;
    public string animalName;

    public Animal(string imgInResources, string animalName)
    {
        this.imgInResources = imgInResources;
        this.animalName = animalName;
    }

}
