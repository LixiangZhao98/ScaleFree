 
    using UnityEngine;
    using UnityEngine.Events;
    [ExecuteInEditMode]
    public class PointCloudRuntime : MonoBehaviour
    {
        public Dataset dataset;
        private Dataset dataset_LastFrame;
        [Tooltip("Use your own function to generate the dataset?")]
        public bool custom=false;
        public bool custom_LastFrame=false;
        public CustomGeneratorEnum customGenerator; 
        public CustomGeneratorEnum customGenerator_LastFrame;  
        public UnityEvent events;
    
        private void OnEnable() 
        {
            custom_LastFrame = custom;
            dataset_LastFrame = dataset;
            customGenerator_LastFrame = customGenerator;
            events?.Invoke(); 
        }
    
        private void Update()
        {
            if (dataset_LastFrame != dataset||customGenerator_LastFrame != customGenerator|custom_LastFrame != custom)
            {
                dataset_LastFrame = dataset;
                customGenerator_LastFrame = customGenerator;
                custom_LastFrame = custom;
                events?.Invoke(); 
            }
        }
        
    
    
    }
    
    
    
     

