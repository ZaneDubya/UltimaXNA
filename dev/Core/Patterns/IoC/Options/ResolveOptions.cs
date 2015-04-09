namespace UltimaXNA.Patterns.IoC
{
    public sealed class ResolveOptions
    {
        private static readonly ResolveOptions _default = new ResolveOptions();
        private static readonly ResolveOptions _failNameNotFoundOnly = new ResolveOptions {NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve};
        private static readonly ResolveOptions _failUnregisteredAndNameNotFound = new ResolveOptions {NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail};
        private static readonly ResolveOptions _failUnregisteredOnly = new ResolveOptions {NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail};

        public static ResolveOptions Default
        {
            get { return _default; }
        }

        public static ResolveOptions FailNameNotFoundOnly
        {
            get { return _failNameNotFoundOnly; }
        }

        public static ResolveOptions FailUnregisteredAndNameNotFound
        {
            get { return _failUnregisteredAndNameNotFound; }
        }

        public static ResolveOptions FailUnregisteredOnly
        {
            get { return _failUnregisteredOnly; }
        }

        private NamedResolutionFailureActions _namedResolutionFailureAction = NamedResolutionFailureActions.Fail;
        private UnregisteredResolutionActions _unregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve;

        public NamedResolutionFailureActions NamedResolutionFailureAction
        {
            get { return _namedResolutionFailureAction; }
            set { _namedResolutionFailureAction = value; }
        }

        public UnregisteredResolutionActions UnregisteredResolutionAction
        {
            get { return _unregisteredResolutionAction; }
            set { _unregisteredResolutionAction = value; }
        }
    }
}