namespace SteeringAssignment_real.Mangers
{
    public class SpawnManager
    {
        private int[] SkeletonsInStage = new int[4];
        private int stage = 0;


        private readonly GameManager _gameManager;

        public SpawnManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            SkeletonsInStage[0] = 1; // Number Of Skeletons in Stage 1
            SkeletonsInStage[1] = 3; // Number Of Skeletons in Stage 2
            SkeletonsInStage[2] = 5; // Etc.. 
            SkeletonsInStage[3] = 1;

            _gameManager.GenerateSkeletons(SkeletonsInStage[stage]);
        }

        public void Update()
        {
            if (AllSkeletonsDead())
            {
                if (stage < SkeletonsInStage.Length)
                {
                    stage++;
                    _gameManager.GenerateSkeletons(stage);
                }
                else
                {
                    // Player Wins
                }

            }
        }

        private bool AllSkeletonsDead()
        {
            bool allSkeletonsDead = true;

            foreach (var skeleton in _gameManager.GetSkeletons())
            {
                if (!skeleton.IsDead())
                {
                    allSkeletonsDead = false;
                }
            }

            return allSkeletonsDead;
        }
    }
}
