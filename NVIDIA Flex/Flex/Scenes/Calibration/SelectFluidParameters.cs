using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // for AssetDatabase to refresh files

public class SelectFluidParameters : MonoBehaviour
{    
    public bool FluidParametersFromFile(NVIDIA.Flex.FlexSourceActor sourceActorScript, string pythonFilePath, bool deleteFile)
    {
        AssetDatabase.Refresh(); // refresh the asset database in case there are new files
        TextAsset pythonFile; // initialise an empty text asset object
        pythonFile = Resources.Load<TextAsset>(pythonFilePath); // try getting the python output file

        if (pythonFile == null) // if there isn't a file yet
        {
            Debug.Log("Please start ParameterOptimisation.py in a terminal.");
            return false; // python output file doesn't exist yet - output false, so "got_file" will still be false
        }

        Debug.Log(pythonFile);
        FluidParameters fluid_params = FluidParameters.CreateFromJSON(pythonFile.text); // create a new "FluidParameters" object, which just stores each parameter as a float

        // Set the fluid parametes in Flex. We do this by accessing the "container" object in the sourceActor script.
        sourceActorScript.container.cohesion = fluid_params.cohesion;
        sourceActorScript.container.surfaceTension = fluid_params.surfaceTension;
        sourceActorScript.container.viscosity = fluid_params.viscosity;
        sourceActorScript.container.adhesion = fluid_params.adhesion;

        if (deleteFile) AssetDatabase.DeleteAsset("Assets/Resources/"+pythonFilePath+".json"); // delete the python output file, now it's been used
        return true; // output true, so "got_file" will be true :)
    }
}
