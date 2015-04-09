namespace UltimaXNA.Patterns.IoC
{
    public sealed class ResolveOptions
    {
        private static readonly ResolveOptions m_default = new ResolveOptions();
        private static readonly ResolveOptions m_failNameNotFoundOnly = new ResolveOptions {NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve};
        private static readonly ResolveOptions m_failUnregisteredAndNameNotFound = new ResolveOptions {NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail};
        private static readonly ResolveOptions m_failUnregisteredOnly = new ResolveOptions {NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail};

        public static ResolveOptions Default
        {
            get { return m_default; }
        }

        public static ResolveOptions FailNameNotFoundOnly
        {
            get { return m_failNameNotFoundOnly; }
        }

        public static ResolveOptions FailUnregisteredAndNameNotFound
        {
            get { return m_failUnregisteredAndNameNotFound; }
        }

        public static ResolveOptions FailUnregisteredOnly
        {
            get { return m_failUnregisteredOnly; }
        }

        private NamedResolutionFailureActions m_namedResolutionFailureAction = NamedResolutionFailureActions.Fail;
        private UnregisteredResolutionActions m_unregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve;

        public NamedResolutionFailureActions NamedResolutionFailureAction
        {
            get { return m_namedResolutionFailureAction; }
            set { m_namedResolutionFailureAction = value; }
        }

        public UnregisteredResolutionActions UnregisteredResolutionAction
        {
            get { return m_unregisteredResolutionAction; }
            set { m_unregisteredResolutionAction = value; }
        }
    }
}