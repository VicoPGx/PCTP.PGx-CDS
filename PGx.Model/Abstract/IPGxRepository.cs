using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGx.Model.Entities;
namespace PGx.Model.Abstract
{
   public interface IPGxRepository
    {     
        IQueryable<VariantLocus> VariantLocus { get;}
        IQueryable<NamedAllele> NamedAllele { get; }
        IQueryable<NamedPhenotype> NamedPhenotype { get; }
       
        IQueryable<PGxGuideline> PGxGuideline { get; }
        IQueryable<DosingGuidence> DosingGuidence { get; }
  
        IQueryable<DefinitionFile> DefinitionFile { get; }
        void SaveChanges();
    }
}
