using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom consisting of child atoms displayed in horizontal row with glueElement between them.
internal interface IRow
{
    // Dummy atom representing atom just before first child atom.
    DummyAtom PreviousAtom { set; }
}
