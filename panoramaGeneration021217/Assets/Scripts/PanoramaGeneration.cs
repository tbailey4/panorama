using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaGeneration : MonoBehaviour {

    public GameObject slices;
    public GameObject lowResSlices;
    public GameObject PanoCamera;
    public CapturePanorama.CapturePanorama pano;


    //Was used for manual testing
    //public KeyCode key = KeyCode.KeypadEnter;
    private int x = 0, y = 0, intx = 5, inty = 5, count=0, count2 = 0;
    public string shape="sc";

    private float camPositionx, camPositiony, camPositionz;
    private bool[,] test = new bool[16, 16];
    private bool[,] test2 = new bool[16, 16];

    // Use this for initialization
    void Start () {

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                test[i, j] = true;
                test2[i, j] = true;
            }
        }

        if (shape == "sc") count=0;

        //need to use lowResSlices later, atm not being used
        //lowResSlices = slices;

        for (int i=0; i < 16; i++)
        {
            for (int j=0; j < 16; j++)
            {

                slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(false);
                lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(false);
                // print(string.Format("disabiling x{0}_y{1}",i,j));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        //Generates terrain in square formation
        if (shape == "s")
        {
            if (!pano.Capturing)//if (Input.GetKeyDown("space"))
            {
                //repositions starting tile
                if (count == 17)
                {
                    count = 0;

                    if (inty == 15)
                    {
                        if (intx == 15)
                        {
                            //currently set to origin position, change to end program once finished
                            inty = 0;
                            intx = 0;
                        }
                        else
                        {
                            intx++;
                            inty = 0;
                        }
                    }
                    else
                    {
                        inty++;
                    }
                }
                camPositionx = (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.x * (intx + 1)) - (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.x / 2);
                camPositiony = 1.2f;
                camPositionz = (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.z * (inty + 1)) - (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.z / 2);

                //repositions panorama camera
                // Vector3 camPosition = (1,1,1);
                PanoCamera.transform.position = (new Vector3(camPositionx, camPositiony, camPositionz));

                //enable terrain slices
                for (int i = intx - count - 1; i <= count + 1 + intx; i++)
                {
                    for (int j = inty - count - 1; j <= count + 1 + inty; j++)
                    {
                        if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)) != null)
                        {
                            slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(true);
                        }

                    }
                }
                //disable terrain slices
                for (int i = intx - count; i <= count + intx; i++)
                {
                    for (int j = inty - count; j <= count + inty; j++)
                    {

                        if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)) != null)
                            slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(false);
                    }
                }

                //captures the panorama
                string panoramaName = string.Format("x{0}_y{1}_{2}", intx, inty, count);
                print(panoramaName);
                pano.CaptureScreenshotAsync(panoramaName);

                count++;

            }
        }
        //generates terrain in semi-circle formations
        else if (shape == "sc")
        {
            //Only executes next step once the previous panorama is fully captured
            if(!pano.Capturing)//if (Input.GetKeyDown("space"))
            {
                
                if (count == 16)
                {
                    count = 0;
                    
                    for (int i = 0; i < 16; i++) {
                        for (int j = 0; j < 16; j++) {
                            test[i, j] = true;
                            test2[i, j] = true;
                        }
                    }
                    if (inty == 15)
                    {
                        if (intx == 15)
                        {
                            //currently set to origin position, change to end program once finished
                            inty = 0;
                            intx = 0;
                        }
                        else
                        {
                            intx++;
                            inty = 0;
                        }
                    }
                    else
                    {
                        inty++;
                    }
                }
                
                //Loads center slice to perform a raycast
                if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)) != null)
                    slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).gameObject.SetActive(true);
                
                //Repositions the camera to the center of the terrain and 1.7 meters above the ground
                camPositionx = (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.x * (intx + 1)) - (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.x / 2);
                camPositiony = 1000f;
                camPositionz = (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.z * (inty + 1)) - (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).GetComponent<Terrain>().terrainData.size.z / 2);
                RaycastHit hit;
                Ray landing = new Ray(new Vector3(camPositionx,camPositiony,camPositionz),Vector3.down);
                Physics.Raycast(landing, out hit);

                if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)) != null)
                    slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).gameObject.SetActive(false);
                //repositions panorama camera
                PanoCamera.transform.position = (new Vector3 (hit.point.x, hit.point.y+1.7f, hit.point.z));

                //disable slices
                for (int i = 0; i <= 15; i++)
                {
                    for (int j = 0; j <= 15; j++)
                    {
                        if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)) != null)
                            slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(false);

                    }
                }

                for (int i = 0; i <= 15; i++)
                {
                    for (int j = 0; j <= 15; j++)
                    {
                        if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)) != null)
                            lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(false);

                    }
                }


                int circle_radius = count;
                float console_ratio = 4.0f / 4.0f;
                float a = console_ratio * circle_radius;
                float b = circle_radius;

                



                for (int y = -circle_radius; y <= circle_radius; y++)
                {
                    for (int x = (int)(-console_ratio * circle_radius); x <= console_ratio * circle_radius; x++)
                    {
                        float d = (x / a) * (x / a) + (y / b) * (y / b);
                        if (d > 0.60 && d < 1.4)
                        {
                            if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", x + intx, y + inty)) != null && test[x + intx, y + inty])
                            {
                                slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", x + intx, y + inty)).gameObject.SetActive(true);
                                test[x + intx, y + inty] = false;
                            }
                        }

                    }
                }

                count2 = count - 1;

                int circle_radius2 = count2;
                float console_ratio2 = 4.0f / 4.0f;
                float a2 = console_ratio2 * circle_radius2;
                float b2 = circle_radius2;

                if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx , inty )) != null)
                {
                    lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).gameObject.SetActive(true);
                }

                for(int i = 0; i <= 15; i++)
                {
                    for (int j = 0; j <= 15; j++)
                    {
                        if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)) != null && !test[i, j])
                            lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", i, j)).gameObject.SetActive(true);

                    }
                }

                /*
                for (int y = -circle_radius2; y <= circle_radius2; y++)
                {
                    for (int x = (int)(-console_ratio2 * circle_radius2); x <= console_ratio2 * circle_radius2; x++)
                    {
                        float d = (x / a2) * (x / a2) + (y / b2) * (y / b2);
                        if (d > 0.60 && d < 1.4)
                        {
                            if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", x + intx, y + inty)) != null && test2[x + intx, y + inty])
                            {
                                lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", x + intx, y + inty)).gameObject.SetActive(true);
                                test2[x + intx, y + inty] = false;
                            }
                        }

                    }
                }*/



                if (count == 1)
                {
                    if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty - 1)) != null)
                    {
                        slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty - 1)).gameObject.SetActive(true);
                        test[intx - 1, inty - 1] = false;
                    }
                    if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty + 1)) != null)
                    {
                        slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty + 1)).gameObject.SetActive(true);
                        test[intx - 1, inty + 1] = false;
                    }
                    if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty - 1)) != null)
                    {
                        slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty - 1)).gameObject.SetActive(true);
                        test[intx + 1, inty - 1] = false;
                    }
                    if (slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty + 1)) != null)
                    {
                        slices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty + 1)).gameObject.SetActive(true);
                        test[intx + 1, inty + 1] = false;
                    }
                    if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)) != null)
                    {
                        lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx, inty)).gameObject.SetActive(true);
                        test[intx, inty] = false;
                    }
                }

                if (count == 2)
                {
                    if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty - 1)) != null)
                    {
                        lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty - 1)).gameObject.SetActive(true);
                    }
                    if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty + 1)) != null)
                    {
                        lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx - 1, inty + 1)).gameObject.SetActive(true);
                    }
                    if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty - 1)) != null)
                    {
                        lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty - 1)).gameObject.SetActive(true);
                    }
                    if (lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty + 1)) != null)
                    {
                        lowResSlices.transform.FindChild(string.Format("Slice_x{0}_y{1}", intx + 1, inty + 1)).gameObject.SetActive(true);
                    }
                }

                //captures the panorama
                string panoramaName = string.Format("x{0}_y{1}_{2}",intx,inty,count);
                print(panoramaName);
                pano.CaptureScreenshotAsync(panoramaName);


                count = count+1;
            }
        }

    }
}
