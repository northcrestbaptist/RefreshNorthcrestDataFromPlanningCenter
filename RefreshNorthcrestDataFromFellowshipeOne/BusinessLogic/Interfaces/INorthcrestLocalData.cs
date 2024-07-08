using Northcrest.Domain.FellowshipOne;

namespace RefreshNorthcrestDataFromFellowshipOne.BusinessLogic.Interfaces
{
    public  interface INorthcrestLocalData
    {
        void RefreshNorthcrestDatabase(IList<Person> northcrestPersonnel);
    }
}
