using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGx.KB.Vcf.model
{
    public interface ReservedProperty {

  
   String getId();

  
   String getDescription();

  
   Type getType();

  Boolean isList();
}
}
