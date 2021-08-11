using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGx.Model.Abstract;
using PGx.Model.Entities;

namespace PGx.Model.Concrete
{
    public class PGxRepository : IPGxRepository
    {
        private PGx_KBEntities context = new PGx_KBEntities();


        public IQueryable<VariantLocus> VariantLocus
        {
            get { return context.VariantLocus; }
        }
        public IQueryable<NamedAllele> NamedAllele
        {
            get { return context.NamedAllele; }
        }
        public IQueryable<NamedPhenotype> NamedPhenotype
        { get { return context.NamedPhenotype; } }

        public IQueryable<PGxGuideline> PGxGuideline
        {
            get { return context.PGxGuideline; }
        }
        public IQueryable<DosingGuidence> DosingGuidence
        {
            get { return context.DosingGuidence; }
        }

        public IQueryable<DefinitionFile> DefinitionFile
        {
            get { return context.DefinitionFile; }
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
