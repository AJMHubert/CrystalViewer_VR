using System;
using System.Collections.Generic;

//
// Created by Jon on 21/08/2015.
//

//
// Created by Jon on 21/08/2015.
//
//Copied unceremoniously by Alex on 15/08/2016

// for file input


public class CIFReader
{
	// constructor that takes filename
	public CIFReader(string filePath)
	{
		// http://stackoverflow.com/questions/2602013/read-whole-ascii-file-into-c-stdstring
		std.ifstream filestream = new std.ifstream(filePath);
		string filecontents;

		filestream.seekg(0, std.ios.end);
		filecontents.reserve(filestream.tellg());
		filestream.seekg(0, std.ios.beg);

		filecontents.assign((std.istreambuf_iterator<sbyte>(filestream)), std.istreambuf_iterator<sbyte>());

		// read in the symmetry elements
		readSymmetryOperations(filecontents);

		// read in atom positions
		readAtomPositions(filecontents);

		// read in unit cell parameters
		readCellGeometry(filecontents);
	}

	// method to return a class instance that will contain the unit cell information
	public UnitCell getUnitCell()
	{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: return UnitCell(cell, atomsites);
		return new UnitCell(new CellGeometry(cell), new List(atomsites));
	}

	// class instance to hold unit cell information
	private List<Symmetry> symmetrylist = new List<Symmetry>();

	private List<AtomSite> atomsites = new List<AtomSite>();

	private CellGeometry cell = new CellGeometry();

	// methods to read
	// 1. atom positions
	// 2. symmetry
	// 3. basis geometry
	private void readAtomPositions(string input)
	{
		// first we need to know where the positions we need are kept (column wise)
		// we do this through the header declaration just after the "loop_"
		std.regex rgxheaders = new std.regex("(?:loop_\\n)((?:_atom_site_\\w+?\\n)+)");
		std.smatch match = new std.smatch();

		if (!std.regex_search(input, match, rgxheaders))
		{
			throw std.runtime_error("Cannot find _atom_site_ block.");
		}

		List<string> headerlines = GlobalMembers.Utilities.split(match[1], '\n');

		// now we can find the values we want and this will be the columns numbers
		// TODO: error handling
		int xcol = GlobalMembers.Utilities.vectorSearch(headerlines, "_atom_site_fract_x");
		int ycol = GlobalMembers.Utilities.vectorSearch(headerlines, "_atom_site_fract_y");
		int zcol = GlobalMembers.Utilities.vectorSearch(headerlines, "_atom_site_fract_z");
		int symbolcol = GlobalMembers.Utilities.vectorSearch(headerlines, "_atom_site_type_symbol");
		// this is not absolutely needed (assume occupancy = 1 otherwise
		int occupancycol = GlobalMembers.Utilities.vectorSearch(headerlines, "_atom_site_occupancy");

		if (xcol >= headerlines.Count)
		{
			throw std.runtime_error("Could not find _atom_site_fract_x.");
		}

		if (ycol >= headerlines.Count)
		{
			throw std.runtime_error("Could not find _atom_site_fract_y.");
		}

		if (zcol >= headerlines.Count)
		{
			throw std.runtime_error("Could not find _atom_site_fract_z.");
		}

		if (symbolcol >= headerlines.Count)
		{
			throw std.runtime_error("Could not find _atom_site_type_symbol.");
		}

		// now search for the lines after the headers
		// construct the regex dynamically (we don't know the number of columns beforehand_
		string positionpattern = "(?:loop_\\n)(?:_atom_site_\\w+?\\n)+((?:\\s*";
		for (int i = 0; i < headerlines.Count - 1; ++i)
		{
			positionpattern += "\\S+[ \\t]+";
		}
		// the last column needs to be different to close off the regex
		positionpattern += "\\S+[ \\t]*\\n)+)";

		std.regex rgxpositions = new std.regex(positionpattern);
		if (!std.regex_search(input, match, rgxpositions))
		{
			throw std.runtime_error("Problem parsing _atom_site_ block.");
		}

		// split the lines
		List<string> atomlines = GlobalMembers.Utilities.split(match[1], '\n');

		List<AtomSite> atomsites_temp = new List<AtomSite>();

		foreach (var line in atomlines)
		{
			// split each line by whitespace (use a regex for this?)
			std.regex rgxcolumns = new std.regex("([^\\s]+)");
			List<string> columns = new List<string>();


			// TODO: this loop is infinite?
			while (std.regex_search(line, match, rgxcolumns))
			{
				// extract column into list of vectors
				for (int j = 1; j < match.size(); ++j)
				{
					columns.Add(match[j]);
				}

				line = match.suffix().str();
			}

			// using the column indices from before, extract the required values
			// will need to remove brackets with errors in
			double x = Convert.ToDouble(GlobalMembers.Utilities.split(columns[xcol], '(')[0]);
			double y = Convert.ToDouble(GlobalMembers.Utilities.split(columns[ycol], '(')[0]);
			double z = Convert.ToDouble(GlobalMembers.Utilities.split(columns[zcol], '(')[0]);
			// name has to be trimmed to remove oxidation state etc..
			string symbol = "";
			std.regex rgxname = new std.regex("([a-zA-Z]{1,2})");
			if (std.regex_search(columns[symbolcol], match, rgxname))
			{
				symbol = match[1];
			}
			// TODO: add handling for when occupancy column does not exist
			double occupancy = Convert.ToDouble(GlobalMembers.Utilities.split(columns[occupancycol], '(')[0]);

			// here we are checking if the atom is on the same site
			Eigen.Vector3d postemp = new Eigen.Vector3d(x, y, z);
			bool isNew = true;

			foreach (var site in atomsites_temp)
			{
				var positions = site.getPositions();
				int ind = GlobalMembers.Utilities.vectorSearch(positions, postemp);
				if (ind >= positions.size())
				{
					continue;
				}
				else
				{
					site.addAtom(symbol, occupancy);
					isNew = false;
					//TODO: think i can break the loop here
				}
			}

			if (isNew)
			{
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: atomsites_temp.push_back(AtomSite(symmetrylist, symbol, x, y, z, occupancy));
				atomsites_temp.Add(new AtomSite(new List(symmetrylist), symbol, x, y, z, occupancy));
			}
		}

		// form new class instance from these and add to vector

		atomsites = new List<AtomSite>(atomsites_temp);
	}

	private void readSymmetryOperations(string input)
	{
		List<Symmetry> symmetrylist_temp = new List<Symmetry>();

		// first we want to know how  columns we have and where the things we want are
		std.regex rgxheaders = new std.regex("(?:loop_\\n)((?:_symmetry_equiv_pos_\\w+?\\n)+)");
		std.smatch match = new std.smatch();

		if (!std.regex_search(input, match, rgxheaders))
		{
			throw std.runtime_error("Cannot find _symmetry_equiv_pos_ block.");
		}

		List<string> headerlines = GlobalMembers.Utilities.split(match[1], '\n');

		int xyzcol = GlobalMembers.Utilities.vectorSearch(headerlines, "_symmetry_equiv_pos_as_xyz");

		if (xyzcol >= headerlines.Count)
		{
			throw std.runtime_error("Could not find _symmetry_equiv_pos_as_xyz.");
		}

		// now extract the block after the headers. This needs to be done dynamically depending on the headers
		string symmetrypattern = "(?:loop_\\n)(?:_symmetry_equiv_pos_\\w+?\\n)+((?:";

		// this is a bit more tricky as all the columns don't have the same regex
		for (int i = 0; i < headerlines.Count; ++i)
		{
			if (i == xyzcol)
			{
				symmetrypattern += "(?:['\"]?.+?,.+?,.+?['\"]?)";
			}
			else
			{
				symmetrypattern += "\\S+";
			}
			if (i != headerlines.Count - 1)
			{
				symmetrypattern += "[ \\t]+";
			}
		}

		symmetrypattern += "\\n)+)";

		std.regex rgxsymmetry = new std.regex(symmetrypattern);
		if (!std.regex_search(input, match, rgxsymmetry))
		{
			throw std.runtime_error("Problem parsing _symmetry_equiv_pos_ block.");
		}

		// split the lines
		List<string> symmetrylines = GlobalMembers.Utilities.split(match[1], '\n');

		// now precess each line to get the symmetry
		foreach (string symmetry in symmetrylines)
		{
			std.regex rgxsymmetry = new std.regex("(['\"].+?['\"]|[\\S]+)");

			List<string> column = new List<string>();

			while (std.regex_search(symmetry, match, rgxsymmetry))
			{
				column.Add(match[1]);
				symmetry = match.suffix().str();
			}

			string symmetryxyz = column[xyzcol];

			// this first regex splits the lines into the individual operations
			std.regex rgxxyz = new std.regex("['\"]*\\s*([^\\s'\"]+)\\s*,\\s*([^\\s'\"]+)\\s*,\\s*([^\\s'\"]+)\\s*['\"]*");

			if (!std.regex_search(symmetryxyz, match, rgxxyz))
			{
				throw std.runtime_error("Problem parsing _symmetry_equiv_pos_ line.");
			}

			var matched = match;
			Symmetry ops = new Symmetry();

			// loop through each operation ( we know there will be 3)
			for (int i = 1; i < 4; ++i)
			{
				// the operation string
				string op = matched[i];

				std.regex rgxoperation = new std.regex("([+-]?[^+-]+)");

	//            if (!std::regex_search(op, match, rgxoperation))
	//                throw std::runtime_error("Problem parsing symmetry operation.");

				List<string> termstrings = new List<string>();

	//            for (int i = 1; i < match.size(); ++i)
	//            {
	//                termstrings.push_back(match[i]);
	//            }
	//            std::regex rgxoperation("([\\+\\-/\\d]*?)([\\+\\-][xyz])([\\+\\-]\\d+/*\\d*){0,1}");

	//            // TODO: maybe check that we have one match first so we can throw an error?
	//            // TODO: check we are reading in 3 operations for each line?

	//            // TODO: preallocate this vector
	//            std::vector<std::vector<std::string>> termstrings;

	//            // match all individual terms in the one operation
	//            // use a while loop now as we expect more than one match per string
				while (std.regex_search(op, match, rgxoperation))
				{
	//                std::vector<std::string> o = {"", "", ""};

	//                for (int j = 1; j < match.size(); ++j)
	//                    o[j - 1] = match[j];

					for (int i = 1; i < match.size(); ++i)
					{
						termstrings.Add(match[i]);
					}

	//                // append to our vector of terms
	//                termstrings.push_back(o);

	//                // trim the string to make sure we don't find the same match
					op = match.suffix().str();
				}

				if (termstrings.Count < 1)
				{
					throw std.runtime_error("Problem parsing symmetry operation.");
				}

				// split terms by +-

//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: ops.setOperation(i-1, termstrings);
				ops.setOperation(i - 1, new List(termstrings));
			}

			// append ops to vector to return
			symmetrylist_temp.Add(ops);
		}

		symmetrylist = new List<Symmetry>(symmetrylist_temp);
	}

	private void readCellGeometry(string input)
	{
		double a = GlobalMembers.Utilities.regexFindDoubleTag(input, "_cell_length_a\\s+([\\d.]+)");
		double b = GlobalMembers.Utilities.regexFindDoubleTag(input, "_cell_length_b\\s+([\\d.]+)");
		double c = GlobalMembers.Utilities.regexFindDoubleTag(input, "_cell_length_c\\s+([\\d.]+)");

		double alpha = GlobalMembers.Utilities.regexFindDoubleTag(input, "_cell_angle_alpha\\s+([\\d.]+)");
		double beta = GlobalMembers.Utilities.regexFindDoubleTag(input, "_cell_angle_beta\\s+([\\d.]+)");
		double gamma = GlobalMembers.Utilities.regexFindDoubleTag(input, "_cell_angle_gamma\\s+([\\d.]+)");

		cell = new CellGeometry(a, b, c, alpha, beta, gamma);
	}

}
