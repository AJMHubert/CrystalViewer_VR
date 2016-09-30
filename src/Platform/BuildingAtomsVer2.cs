using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


public class BuildingAtomsVer2 : MonoBehaviour
{
    private GameObject atom;
    private ArrayList myNodes;

    // Use this for initialization
    void Start()
    {
        //find number of lines in file
        int counter = 0;
        int numberoflines;
        string linedummy;
	

        string[] periodictable = {"H","He" ,"Li", "Be", "B", "C",  "N",  "O",  "F",
                "Ne", "Na", "Mg", "Al", "Si", "P",  "S",  "Cl", "Ar", "K",
                "Ca", "Sc", "Ti", "V",  "Cr", "Mn", "Fe", "Co", "Ni", "Cu",
            "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr", "Rb", "Sr", "Y",  "Zr",
            "Nb", "Mo ", "Tc", "Ru", " Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb",
            "Te", "I",  "Xe", "Cs", "Ba", "La", "Ce", "Pr", "Nd", "Pm", "Sm",
            "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu", "Hf", "Ta", "W",
            "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At","Rn",
            "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf",
            "Es", "Fm", "Md", "No", "Lr", "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds",
            "Rg", "Cn", "Uut","Uuq","Uup","Uuh","Uus","Uuo"};

        //Read in cell length coordinates

        // Read the file and count the number of lines.
        System.IO.StreamReader file =
        //new System.IO.StreamReader(@"C:\Users\Alex\Documents\Play\Si.txt");
        new System.IO.StreamReader(@"C:\Users\JJ\Documents\Alex_crystal_project\CrystalViewer_VR_scripts\Demo\Pb2ScTaO6-R3-a+a+a+-antiferro.cif");
        while ((linedummy = file.ReadLine()) != null)
        {
            counter++;
        }
        numberoflines = counter;
        file.Close();
        Debug.Log("There were " + numberoflines + "lines");

        //Initialise column number for atomic coordinate data in .cif file
        int occupancycolumn = 0,
            fractxcolumn = 0,
            fractycolumn = 0,
            fractzcolumn = 0, 
            elementcolumn = 0;

        float CellLengthA=0.0F, CellLengthB = 0.0F, CellLengthC = 0.0F, 
            CellAlpha = 0.0F, CellBeta = 0.0F, CellGamma = 0.0F;

        // Read in unit cell characteristics
        string[] lines = System.IO.File.ReadAllLines(@"C:\Users\JJ\Documents\Alex_crystal_project\CrystalViewer_VR_scripts\Demo\Pb2ScTaO6-R3-a+a+a+-antiferro.cif");
        foreach (string line1 in lines)
        {
            string[] text = System.Text.RegularExpressions.Regex.Split(line1, @"\s+");

            switch (text[0])
            {
                case "_cell_length_a":
                    string CellLengthAString = text[1];
                    CellLengthAString = CellLengthAString.Replace("(", "");
                    CellLengthAString = CellLengthAString.Replace(")", "");
                    Debug.Log("cell length A" + CellLengthAString);
                    CellLengthA = Convert.ToSingle(CellLengthAString);
                    break;
                case "_cell_length_b":
                    string CellLengthBString = text[1];
                    CellLengthBString = CellLengthBString.Replace("(", "");
                    CellLengthBString = CellLengthBString.Replace(")", "");
                    Debug.Log("cell length A" + CellLengthBString );
                    CellLengthB = Convert.ToSingle(CellLengthBString);
                    break;
                case "_cell_length_c":
                    string CellLengthCString = text[1];
                    CellLengthCString = CellLengthCString.Replace("(", "");
                    CellLengthCString = CellLengthCString.Replace(")", "");
                    Debug.Log("cell length C" + CellLengthCString);
                    CellLengthC = Convert.ToSingle(CellLengthCString);
                    break;
                case "_cell_angle_alpha":
                    string CellAlphaString = text[1];
                    CellAlphaString = CellAlphaString.Replace("(", "");
                    CellAlphaString = CellAlphaString.Replace(")", "");
                    Debug.Log("cell length Alpha" + CellAlphaString);
                    CellAlpha = Convert.ToSingle(CellAlphaString);
                    break;
                case "_cell_angle_beta":
                    string CellBetaString = text[1];
                    CellBetaString = CellBetaString.Replace("(", "");
                    CellBetaString = CellBetaString.Replace(")", "");
                    CellBeta = Convert.ToSingle(CellBetaString);
                    break;
                case "_cell_angle_gamma":
                    string CellGammaString = text[1];
                    CellGammaString = CellGammaString.Replace("(", "");
                    CellGammaString = CellGammaString.Replace(")", "");
                    CellGamma = Convert.ToSingle(CellGammaString);
                    break;
                default:
                    
                    break;
            }
        }
        file.Close();

        //add in  end loop references for easier read in
        int i;
        string[] newlines = new string[numberoflines + 1];
        int counter1 = 0;
        for (i = 0; i < numberoflines; i++)
        {
            if (lines[i].Contains("loop_") & counter1 == 0)
            {
                newlines[i] = lines[i];
                counter1 = 1;
            }
            else if (lines[i].Contains("loop_") & counter1 > 0)
            {
                newlines[i - 1] = lines[i - 1] + " _endloop";
                newlines[i] = lines[i];
                counter1++;
            }
            else
            {
                newlines[i] = lines[i];
            }
        }
        newlines[numberoflines] = "_endloop";

        //looping zone switches/identifiers
        int loopbegin = 0, loopbegin1 = 1000;
        int loopend = 0, loopend1 = 0;
        int jnd;
        int symmetryxyzswitch = 0, symmetryid = 0;
        int fraccounter = 0;
        int loopduration1 = 0;
        int loopduration = 0;
        int Symcounter = 0;

        //find the loop containing symmetry elements and assign variables
        for (i = 0; i < numberoflines; i++)
        {
            if (newlines[i].Contains("_symmetry_equiv_pos_as_xyz"))
            {
                symmetryxyzswitch = 1;
            }
            if (newlines[i].Contains("_symmetry_equiv_pos_site_id"))
            {
                symmetryid = 1;
            }
            if (symmetryxyzswitch == 1 & symmetryid == 1 & loopbegin == 0)
            {
                loopbegin = i + 1;
                Debug.Log("loopbegin " + loopbegin);
            }
            if (symmetryxyzswitch == 1 & symmetryid == 0 & loopbegin == 0)
            {
                loopbegin = i + 1;
                Debug.Log("loopbegin " + loopbegin);
            }
            if (loopbegin > 0 & newlines[i].Contains("_endloop"))
            {
                loopend = i + 1;
                loopduration = loopend - loopbegin;
            }
        }

        //Initialise symmetries and their Id
        string[] Symstring = new string[loopduration];
        string[] idstring = new string[loopduration];

        //loop over symmetry elements, split string on ' character. 
        //Save in above variables
        for (jnd = loopbegin; jnd < loopend; jnd++)
        {
            string[] text = System.Text.RegularExpressions.Regex.Split(newlines[jnd], "\'");
            Symstring[Symcounter] = text[1];
            idstring[Symcounter] = text[0];
            Symcounter++;
        }
        
        //Matrix & Vector forms of symmetry elements
        double[,,] SymMatrix = new double[Symcounter, 3, 3];
        double[,] SymVector = new double[Symcounter, 3];

        //Read into symmetry matrix and vector. Careful with negative values
        for (i=0;i<Symcounter; i++)
        {
            char[] Delimeters = { ',' };
            string[] text= Symstring[i].Split(Delimeters);

            for (jnd = 0; jnd < 3; jnd++)
            {
                if (text[jnd].Contains("x"))
                {
                    int xpos = text[jnd].IndexOf("x");
                    string minussignx = "";
                    if (xpos == 0)
                    {
                         minussignx = "+";
                    }
                    else
                    {
                        minussignx = text[jnd].Substring(xpos - 1, xpos - 1);
                    }
                if (minussignx.Contains("-"))
                    {
                        SymMatrix[i, jnd, 0] = -1.0;
                    }
                    else
                    {
                        SymMatrix[i, jnd, 0] = 1.0;
                    }
                                    
                }

                if (text[jnd].Contains("y"))
                {
                    int ypos = text[jnd].IndexOf("y");
                  
                    string minussigny = "";
                    if (ypos == 0)
                    {
                        minussigny = "+";
                    }
                    else
                    {
                        minussigny = text[jnd].Substring(ypos - 1, ypos - 1);
                    }

                    if (minussigny.Contains("-"))
                    {
                        SymMatrix[i, jnd, 1] = -1.0;
                    }
                    else
                    {
                        SymMatrix[i, jnd, 1] = 1.0;
                    }

                }

                if (text[jnd].Contains("z"))
                {
                    int zpos = text[jnd].IndexOf("z");
                    string minussignz = "";

                    if (zpos == 0)
                    {
                        minussignz = "+";
                    }
                    else
                    {
                        minussignz = text[jnd].Substring(zpos - 1, zpos - 1);
                    }


                    if (minussignz.Contains("-"))
                    {
                        SymMatrix[i, jnd, 2] = -1.0;
                    }
                    else
                    {
                        SymMatrix[i, jnd, 2] = 1.0;
                    }
                        
                }
                //Vector read in (fractional).
                if  (text[jnd].Contains("/"))
                {
                    int bracket = text[jnd].IndexOf("/");
                    string posneg= text[jnd].Substring(bracket - 2, 1);  
                    string nomstring = text[jnd].Substring(bracket - 1, 1);
                    string denomstring = text[jnd].Substring(bracket + 1, 1);
                    double nom = Convert.ToDouble(nomstring);
                    double denom = Convert.ToDouble(denomstring);

                    if (posneg.Contains("-")) { SymVector[i, jnd] = -(nom / denom); }
                    if (posneg.Contains("+")) { SymVector[i, jnd] = nom / denom; }

                }
            }           
                
        }

         
        //find second loop - assign atomic position variables
        for (i = 0; i < numberoflines + 1; i++)
        {
            if (newlines[i].Contains("_atom_site_label"))
            {
                loopbegin1 = i + 1;
            }
            if (newlines[i].Contains("_atom_site_type_symbol"))
            {
                elementcolumn = (i - loopbegin1) + 1;
                Debug.Log("elementcolumn = " + elementcolumn);
            }
            if (newlines[i].Contains("_atom_site_occupancy"))
            {
                occupancycolumn = (i - loopbegin1) + 1;
                Debug.Log("occupancycolumn = " + occupancycolumn);
            }
            if (newlines[i].Contains("_atom_site_fract_x"))
            {
                fractxcolumn = (i - loopbegin1) + 1;
                Debug.Log("fractxcolumn = " + fractxcolumn);
            }
            if (newlines[i].Contains("_atom_site_fract_y"))
            {
                fractycolumn = (i - loopbegin1) + 1;
                Debug.Log("fractycolumn = " + fractycolumn);
            }
            if (newlines[i].Contains("_atom_site_fract_z"))
            {
                fractzcolumn = (i - loopbegin1) + 1;
                Debug.Log("fractzcolumn = " + fractzcolumn);
            }

            if (i > loopbegin1 & newlines[i].Contains("_endloop"))
            {
                loopend1 = i + 1;

                loopduration1 = loopend1 - loopbegin1;
            }
        }

        string[] atomsitelabel = new string[loopduration1];
        string[] occupancy = new string[loopduration1];
        string[] fractx = new string[loopduration1];
        string[] fracty = new string[loopduration1];
        string[] fractz = new string[loopduration1];
        string[] element = new string[loopduration1];

        //save values into variables
        for (jnd = loopbegin1; jnd < loopend1; jnd++)
        {   
            if (newlines[jnd].Contains("."))
            {
                string[] text = System.Text.RegularExpressions.Regex.Split(newlines[jnd], @"\s+");

                atomsitelabel[fraccounter] = text[0].Trim();
                element[fraccounter] = text[elementcolumn].Trim();
                occupancy[fraccounter] = text[occupancycolumn];
                fractx[fraccounter] = text[fractxcolumn];
                fracty[fraccounter] = text[fractycolumn];
                fractz[fraccounter] = text[fractzcolumn];

                fraccounter++;
            }
        }
        Debug.Log("counter4 = " + fraccounter);

        
        double[,] xyzfrac = new double[fraccounter*Symcounter, 3];
        double[,] xyzcoord = new double[fraccounter*Symcounter, 3];
        float[,] xyzfloat = new float[fraccounter*Symcounter, 3];

        //Convert string to double values (fractional coordinates)
        for (jnd=0;jnd<fraccounter;jnd++)
        {
            xyzfrac[jnd, 0] = Convert.ToSingle(fractx[jnd]);
            xyzfrac[jnd, 1] = Convert.ToSingle(fracty[jnd]);
            xyzfrac[jnd, 2] = Convert.ToSingle(fractz[jnd]);
  
            Debug.Log("xyzcoordx = " + xyzfrac[jnd, 0]);
            Debug.Log("xyzcoordy = " + xyzfrac[jnd, 1]);
            Debug.Log("xyzcoordz = " + xyzfrac[jnd, 2]);
        }

        double[,] fullfrac = new double[Symcounter * fraccounter, 3];
        double[] fullfractemp = new double[3];
        string[] AtomName = new string[Symcounter * fraccounter];
        double[] FullOccupancy = new double[Symcounter * fraccounter];
        int fullindex = 0;

        for (i=0; i<Symcounter;i++)
        {
            for (jnd=0;jnd< fraccounter;jnd++)
            {
                if (fraccounter == 1)
                {
                    fullindex = i;    
                }
                else
                {
                   fullindex = ((fraccounter - 1) * i) + jnd;
                }
                               
                Debug.Log("fullindex" + fullindex);
                Debug.Log("FracCounter  " + fraccounter);
                Debug.Log("SymCounter  " + Symcounter);
            
                for (int knd=0;knd<3;knd++)
                {    
                    fullfrac[fullindex, knd] = (SymMatrix[i, knd, 0] * xyzfrac[jnd, 0] + SymMatrix[i, knd, 1] * xyzfrac[jnd, 1] +
                        SymMatrix[i, knd, 2] * xyzfrac[jnd, 2])+SymVector[i,knd];
                    fullfractemp[knd] = fullfrac[fullindex, knd];
                    Debug.Log("SymCounter  " + Symcounter);
                    if (fullfrac[fullindex, knd]<0)
                    {
                        fullfrac[fullindex, knd] =
                            fullfrac[fullindex, knd] + 1.0;
                    }
                }
                
                AtomName[fullindex] = element[jnd];
                FullOccupancy[fullindex] = Convert.ToDouble(occupancy[jnd]);

                Debug.Log("fullfrac =  " + AtomName[fullindex] + " " + fullfrac[fullindex,0]
                    + " " + fullfrac[fullindex,1] + " " + fullfrac[fullindex,2]);
            
            }
        }

        for (i = 0; i < (Symcounter * fraccounter) - 2; i++)
        {
            for (jnd = 0; jnd < 3; jnd++)
            {
                if (fullfrac[i, jnd] < 0)
                {
                    fullfrac[i, jnd] = fullfrac[i, jnd] + 1;
                }
                if (fullfrac[i, jnd] >= 1)
                {
                    fullfrac[i, jnd] = fullfrac[i, jnd] - 1;
                }

            }
        }

        string[] UniqueAtomName = new string[Symcounter * fraccounter];
        double[,] Uniquefrac = new double[Symcounter * fraccounter, 3];
        int uniquenum = 0;
        int UniqueSwitch = 0;

        UniqueAtomName[0] = AtomName[0];
        Debug.Log("AtomName" + AtomName[0]);
        Uniquefrac[0, 0] = fullfrac[0, 0];
        Uniquefrac[0, 1] = fullfrac[0, 1];
        Uniquefrac[0, 2] = fullfrac[0, 2];


        for (i=1;i<(Symcounter* fraccounter)-2;i++)
        {
            for (jnd=0;jnd<uniquenum+1;jnd++)
            {
                if (fullfrac[i,0]==Uniquefrac[jnd,0] &
                    fullfrac[i, 1] == Uniquefrac[jnd, 1] &
                    fullfrac[i, 2] == Uniquefrac[jnd, 2] &
                    AtomName[i]==UniqueAtomName[jnd])
                {
                    UniqueSwitch = 0;
                    break;

                }
                UniqueSwitch = 1;
            }
            Debug.Log("Uniqueswitch= " + UniqueSwitch);
            if(UniqueSwitch==1)
            {
                uniquenum++;
                for (int knd=0;knd<3;knd++)
                {
                    Uniquefrac[uniquenum, knd] = fullfrac[i, knd];
                    UniqueAtomName[uniquenum] = AtomName[i];
                }
            }

        }
        double[,] Uniquefractot = new double[uniquenum, 3];
        for (i=0;i< uniquenum;i++)
        {
            Uniquefractot[i, 0] = Uniquefrac[i, 0];
            Uniquefractot[i, 1] = Uniquefrac[i, 1];
            Uniquefractot[i, 2] = Uniquefrac[i, 2];
            Debug.Log("Uniquefractot = " + Uniquefractot[i, 0] + " " + Uniquefractot[i, 1] + " " + Uniquefractot[i, 2]);
        }


        double cosbeta = Math.Cos(CellBeta*(Math.PI/180.0));
        double cosalpha = Math.Cos(CellAlpha*(Math.PI/180.0));
        double cosgamma = Math.Cos(CellGamma*(Math.PI/180.0));
        double singamma = Math.Sin(CellGamma*(Math.PI/180.0));

        double vcoord = 1 - (cosalpha * cosalpha) - (cosbeta * cosbeta) - (cosgamma * cosgamma)
            + (2 * cosalpha * cosbeta * cosgamma);
        double vcoord1 = Math.Sqrt(vcoord);

        double[,] transmatrix = new double[3, 3] { { CellLengthA, CellLengthB * cosgamma, CellLengthC*cosbeta }
            , {0,CellLengthB*singamma,CellLengthC*((cosalpha-cosbeta*cosgamma)/singamma) }
            , {0,0,CellLengthC*(vcoord1/singamma) } };

        for (jnd = 0; jnd < Symcounter*fraccounter; jnd++)
        {
            for (i = 0; i < 3; i++)
            {

                xyzcoord[jnd, i] = (transmatrix[i, 0] * Uniquefrac[jnd, 0]) +
                    (transmatrix[i, 1] * Uniquefrac[jnd, 1])
                    + (transmatrix[i, 2] * Uniquefrac[jnd, 2]);
                xyzfloat[jnd,i] = Convert.ToSingle(xyzcoord[jnd,i]);

                

                Debug.Log("xyzfloat" + xyzfloat[jnd, i]);
               
            }
            Debug.Log("Unique frac" + " " + Uniquefrac[jnd, 0]
                    + " " + Uniquefrac[jnd, 1] + " " + Uniquefrac[jnd, 2]);
        }



        myNodes = new ArrayList();
        int strIndex;
        int strNumber;
  
        for (i = 0; i < uniquenum; i++)
        {
            strIndex = 0;
          
            for (strNumber = 0; strNumber < 119; strNumber++)
            {
                Debug.Log("element[i] " + UniqueAtomName[i]);
                strIndex = periodictable[strNumber].IndexOf(UniqueAtomName[i]);
                if (strIndex >= 0)
                    break;
               
            }

           
            double doublestrFloat = Convert.ToDouble(strNumber);
            double strLog = (Math.Log(doublestrFloat)+1)/10;
            float strFloat = Convert.ToSingle(strLog);
            atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atom.transform.position = new Vector3(xyzfloat[i, 0], xyzfloat[i, 1], xyzfloat[i, 2]);
            atom.transform.localScale = new Vector3(strFloat, strFloat, strFloat);
            

            myNodes.Add(atom);

        }

        //1. Find lowest atomic position in z, add in plane at a certain distance below that
        //2. Set up camera coordinate system
        //3. construct a mathematical sphere of radius r based on furthest atomic position
        //4. Need

    }




    // Update is called once per frame
    void Update()
    {

    }
}
// Debug.Log(" here i = " + i + " jnd = " + jnd + " knd = " +knd +  " check ");
//Debug.Log("xyzfrac [jnd,0] = " + xyzfrac[jnd, 0]);
//Debug.Log("SymMatrix[i, knd, 1] = " + SymMatrix[i, knd, 1]);
// Debug.Log("SymVector[i,knd] = " + SymVector[i, knd]);
// Debug.Log("fullfrac[fullindex, knd] = " + fullfrac[fullindex, knd]);
