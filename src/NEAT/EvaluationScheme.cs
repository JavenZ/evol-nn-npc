    using SharpNeat;

    #pragma warning disable

    public sealed class EvaluationScheme : IBlackBoxEvaluationScheme<double>
    {
        public GamePool GamePool {set; get;}

        /// <inheritdoc/>
        public int InputCount => 3;

        /// <inheritdoc/>
        public int OutputCount => 1;

        /// <inheritdoc/>
        public bool IsDeterministic => true;

        /// <inheritdoc/>
        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        /// <inheritdoc/>
        public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

        /// <inheritdoc/>
        public bool EvaluatorsHaveState => false;

        /// <inheritdoc/>
        public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
        {
            return new Evaluator()
            {
                GamePool=GamePool,
            };
        }

        /// <inheritdoc/>
        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return (fitnessInfo.PrimaryFitness >= 10);
        }
    }