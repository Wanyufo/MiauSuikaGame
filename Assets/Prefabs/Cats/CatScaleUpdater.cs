using System;
using System.Collections.Generic;
using UnityEngine;


// Script for development purposes. It is used to update the scale of the cat prefab, which are only used for visual help during development. The actual cat's scales will be set during runtime.

namespace Prefabs.Cats
{
    public class CatScaleUpdater : MonoBehaviour
    {
        [Header("This is not a runtime script. it is used to set the scales of the prefabs once during development.")]
        // List of all cat prefabs, serialized
        [SerializeField]
        private bool updateScales = false;

        [SerializeField] private List<GameObject> catPrefabs;
        private List<GameObject> instantiatedCats = new List<GameObject>();
        [SerializeField, Range(0.1f, 2)] private float scaleOfSmallestCat = 1;
        [SerializeField, Range(1, 2)] float scaleFactor = 1;
        [SerializeField, Range(0, 2)] float additiveScale = 0;


        private void SetCatScales()
        {
            foreach (GameObject cat in instantiatedCats)
            {
                DestroyImmediate(cat);
            }

            instantiatedCats.Clear();
            // Set the scale of each cat prefab
            for (int i = 0; i < catPrefabs.Count; i++)
            {
                int catNr = i + 1;
                double scale = scaleOfSmallestCat * Math.Pow(scaleFactor, i) + additiveScale * i;
                Debug.Log("Cat " + catNr + " scale: " + scale);
                catPrefabs[i].transform.localScale = new Vector3((float) scale, (float) scale, (float) scale);
                SetMass(catPrefabs[i], scale);
                instantiatedCats.Add(Instantiate(catPrefabs[i],
                    new Vector3((float) (i * (scale / 2)), this.transform.position.y, 0),
                    Quaternion.identity));
                
            }
        }


        private void SetMass(GameObject cat, double scale)
        {
            // Calculate the mass of the cat based on its scale
            Rigidbody2D rigi = cat.GetComponent<Rigidbody2D>();
            rigi.useAutoMass = false;
            rigi.mass = (float) (scale * 3);
        }
        

        private void OnValidate()
        {
            if (updateScales) SetCatScales();
            updateScales = false;
        }
    }
}