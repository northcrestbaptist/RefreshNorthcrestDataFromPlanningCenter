using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;


namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic
{
    public class TransformData : ITransformData
    {
        public void ExecuteDataCorrections(Sermon sermon)
        {
            if (!string.IsNullOrEmpty(sermon.Title))
            {
                sermon.Title = TransformTitle(sermon.Title);
            }
            if (!string.IsNullOrEmpty(sermon.Speaker))
            {
                sermon.Speaker = TransformSpeakerName(sermon.Speaker);
            }
        }

        private string TransformTitle(string title)
        {
            string transformedTitle;
            if (title.StartsWith("Sermon -"))
            {
                transformedTitle = title.Substring(8).Trim();
            }
            else if (title.StartsWith("Sermon-"))
            {
                transformedTitle = title.Substring(7).Trim();
            }
            else if (title.StartsWith("Sermon"))
            {
                transformedTitle = title.Substring(6).Trim();
            }
            else 
            {
                transformedTitle = title != null ? title.Trim() : title;
            }

            if(transformedTitle != null && transformedTitle.Trim().Length == 0)
            {
                transformedTitle = null;
            }

            return transformedTitle;
        }


        private string TransformSpeakerName(string speakerName)
        {
            speakerName = RemoveBroFromSpeaker(speakerName);
            speakerName = ConsolidateMultipleNameVersions(speakerName);
            if (speakerName != null && speakerName.Trim().Length == 0)
            {
                speakerName = null;
            }
            return speakerName;
        }

        private string RemoveBroFromSpeaker(string speaker)
        {
            string transformedSpeaker;
            if (speaker.StartsWith("Bro."))
            {
                transformedSpeaker = speaker.Substring(4).Trim();
            }
            else if (speaker.StartsWith("Bro"))
            {
                transformedSpeaker = speaker.Substring(3).Trim();
            }
            else
            {
                transformedSpeaker = speaker != null ? speaker.Trim() : speaker;
            }

            return transformedSpeaker;
        }

        private string ConsolidateMultipleNameVersions(string name)
        {
            string transformedName;
            switch (name.Trim())
            {
                case "Dan Lanier":
                case "Dr. Dan Lanier":
                case "Dr Dan Lanier":
                case "D. Dan Lanier":
                case "Dr, Dan Lanier":
                case "Dr.Dan Lanier":
                case "Dr. Dan Lanir":
                case "Dr. Dan :Lanier":
                case "Dr. Dan Danier":
                case "Dr. Da Lanier":
                case "Dr. Dan Lanier, senior pastor Northcrest Baptist":
                    transformedName = "Dr. Dan Lanier";
                    break;
                case "Barry Chesney | Discipleship Pastor | Valleydale Church, Birmingham, AL":
                    transformedName = "Barry Chesney";
                    break;
                case "Casie Paige":
                    transformedName = "Casie Page";
                    break;
                case "D. Jim Futral":
                    transformedName = "Dr. Jim Futral";
                    break;
                case "Greg Massey":
                    transformedName = "Dr. Greg Massey";
                    break;
                case "Herb Reavis":
                    transformedName = "Dr. Herb Reavis";
                    break;
                case "Jeff LaBorg":
                    transformedName = "Dr. Jeff LaBorg";
                    break;
                case "John M Davis":
                    transformedName = "John M. Davis";
                    break;
                case "Junior Hill":
                    transformedName = "Dr. Junior Hill";
                    break;
                case "Kevin Hamm":
                    transformedName = "Dr. Kevin Hamm";
                    break;
                case "Sammy Gilbreath":
                    transformedName = "Dr. Sammy Gilbreath";
                    break;
                case "Wade Phillips":
                case "Wde Phillips":
                    transformedName = "Dr. Wade Phillips";
                    break;
                case "Warren Haynes":
                    transformedName = "Dr. Warren Hayes";
                    break;
                case "Mic #7 & #8 for prayers for Lord's supper":
                    transformedName = null;
                    break;
                case "Wade Ricks - FBC Collinsville":
                    transformedName = "Wade Ricks";
                    break;
                case "Rick Corum":
                    transformedName = "Rick Coram";
                    break;
                default:
                    transformedName = name;
                    break;
            }

            return transformedName;
        }
    }
}
