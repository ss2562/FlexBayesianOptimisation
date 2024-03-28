using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;
using System.IO;

public class CaptureParticlePositions : MonoBehaviour
{
    public void SaveParticlePositions(NVIDIA.Flex.FlexSourceActor sourceActorScript, string filename)
    {
        Vector4[] m_particleArray = GetParticlePositions(sourceActorScript); // get the positions of all the particles
        
        string filenameComplete = filename + ".csv";
        StreamWriter writer = new StreamWriter(filenameComplete); // "StreamWriter" object is used to save the CSV file

        // Write fluid parameter description as first 2 lines of the CSV file
        writer.WriteLine("Cohesion, Surface tension, Viscosity, Adhesion");
        float[] fluid_description_array = new float[] { sourceActorScript.container.cohesion,
                                                        sourceActorScript.container.surfaceTension,
                                                        sourceActorScript.container.viscosity,
                                                        sourceActorScript.container.adhesion };
        string fluid_description = string.Join(",", fluid_description_array);
        writer.WriteLine(fluid_description);
        
        writer.WriteLine("x, y, z"); // write column headings in the 3rd line of CSV file

        for (int i=0; i<sourceActorScript.container.maxParticles; i++){ // iterate over all the particles
            Vector4 pos = m_particleArray[i]; // get position
            string line = pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString(); // convert position to string separated by commas
            writer.WriteLine(line); // write line to CSV file
        }
        writer.Close(); // close the writer - otherwise we won't be able to access the CSV file
    }

    public Vector4[] GetParticlePositions(NVIDIA.Flex.FlexSourceActor sourceActorScript)
    {
        FlexExt.ParticleData particleData = FlexExt.MapParticleData(sourceActorScript.container.handle); // Get pointer to where particle positions are stored
        Vector4[] m_particleArray = new Vector4[sourceActorScript.container.maxParticles]; // Create empty array to store particle positions in
        FlexUtils.FastCopy(particleData.particles, m_particleArray); // Copy particle positions into the empty array

        FlexExt.UnmapParticleData(sourceActorScript.container.handle); // Unlink the pointer from the GPU to allow computation to continue (I think)

        return m_particleArray;
    }

    public Vector3[] GetParticleVelocities(NVIDIA.Flex.FlexSourceActor sourceActorScript)
    {
        FlexExt.ParticleData particleData = FlexExt.MapParticleData(sourceActorScript.container.handle); // Get pointer to where particle positions are stored
        Vector3[] m_velocityArray = new Vector3[sourceActorScript.container.maxParticles]; // Create empty array to store particle positions in
        FlexUtils.FastCopy(particleData.velocities, m_velocityArray); // Copy particle positions into the empty array

        FlexExt.UnmapParticleData(sourceActorScript.container.handle); // Unlink the pointer from the GPU to allow computation to continue (I think)

        return m_velocityArray;
    }
}
