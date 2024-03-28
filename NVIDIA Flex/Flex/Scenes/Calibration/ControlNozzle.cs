using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

public class ControlNozzle : MonoBehaviour
{

    public void SetNozzleFlow(NVIDIA.Flex.FlexSourceActor sourceActorScript)
    {
        // volume and dispensing time are predefined
        float fluid_volume_ml = 4.8f;
        float dispensing_time = 15.0f;
        float flow_rate_ms = fluid_volume_ml / dispensing_time;

        // from sphere packing equation
        float particles_per_ml = 1.0f / (4 * Mathf.Sqrt(2) * Mathf.Pow(sourceActorScript.container.fluidRest*0.52f, 3));
        float flow_rate = flow_rate_ms * particles_per_ml * Time.fixedDeltaTime; // desired flow rate in particles per frame - converted from ml/s (ml/s * particles/ml * s/frame)
        
        int num_particles_per_layer = 5; // depends on shape of layer

        for (int n=1; n<5; n++){ // dT should be a multiple of (num_particles_per_layer / particles_per_ml / flow_rate_ms)
            float ideal_deltaTime = n * num_particles_per_layer / particles_per_ml / flow_rate_ms; // to achieve the desired flow_rate, we need to set the dT properly, as 5 particles are released per dT time.
            Debug.Log("The ideal deltaTime is " + ideal_deltaTime);
        }
        int num_layers = (int)Mathf.Round( flow_rate / num_particles_per_layer ); // get whole number of layers - in future could implement a way to handle the excess, e.g. always round down & keep track of how many were skipped.
        int num_particles = num_particles_per_layer * num_layers;

        float dist_travelled_in_first_frame = 0.5f * 981.0f * Time.fixedDeltaTime * Time.fixedDeltaTime;
        float layer_spacing = dist_travelled_in_first_frame / num_layers; // space the particles in 1 batch evenly

        float spacing = 0.08f; // spacing in the horizontal plane i.e. between particles in a layer

        // initialise the position and movement direction of a batch of particles, relative to the fluid source object
        Vector3[] nozzlePosNew = new Vector3[num_particles];
        Vector3[] nozzleDirNew = new Vector3[num_particles];
        for (int i=0; i<num_layers; i++){
            nozzlePosNew[i*num_particles_per_layer] = new Vector3(-spacing, -layer_spacing*i, 0.0f);
            nozzlePosNew[i*num_particles_per_layer+1] = new Vector3(spacing, -layer_spacing*i, 0.0f);
            nozzlePosNew[i*num_particles_per_layer+2] = new Vector3(0.0f, -layer_spacing*i, -spacing);
            nozzlePosNew[i*num_particles_per_layer+3] = new Vector3(0.0f, -layer_spacing*i, spacing);
            nozzlePosNew[i*num_particles_per_layer+4] = new Vector3(0.0f, -layer_spacing*i, 0.0f);

            for (int j=0; j<5; j++){
                nozzleDirNew[i+j] = new Vector3(0.0f, -1.0f, 0.0f);
            }
        }
        sourceActorScript.asset.nozzlePositions = nozzlePosNew;
        sourceActorScript.asset.nozzleDirections = nozzleDirNew;

        // TO DO: set number of particles in container as num_particles from this script (this might be better for computation?)
    }

    // Update is called once per frame
    public bool MoveNozzle(NVIDIA.Flex.FlexSourceActor sourceActorScript, GameObject nozzle, GameObject nozzleTarget, float nozzleSpeed, float dT)
    {
        Vector3 error = (nozzleTarget.transform.position - nozzle.transform.position); // error between target and current nozzle position
        if (error.magnitude < nozzleSpeed * dT) { // reached target
            return true;
        }
        nozzle.transform.position += error.normalized * nozzleSpeed * dT; // move nozzle (currently just a proportional controller)
        return false; // not reached target
    }
}
