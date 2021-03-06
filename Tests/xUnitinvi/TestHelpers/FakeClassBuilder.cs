﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using FakeItEasy;
using Tweetinvi.Core.Injectinvi;

namespace xUnitinvi.TestHelpers
{
    [ExcludeFromCodeCoverage]
    public class FakeClassBuilder<T> where T : class
    {
        private readonly IContainer _container;
        private readonly ContainerBuilder _containerBuilder;
        private readonly FakeRepository _fakeRepository;

        public FakeClassBuilder(params string[] parametersToIgnore)
        {
            _fakeRepository = new FakeRepository();
            _containerBuilder = new ContainerBuilder();
            InitializeContainer(parametersToIgnore);
            _container = _containerBuilder.Build();
        }

        private void InitializeContainer(params string[] parametersToIgnore)
        {
            var constructor = GetInjectionConstructor();
            var fakeType = typeof(Fake<>);

            foreach (var parameter in constructor.GetParameters())
            {
                if (parametersToIgnore.Any(x => x == parameter.Name))
                {
                    continue;
                }

                var t = parameter.ParameterType;
                if (!t.IsValueType)
                {
                    var parameterFakeType = fakeType.MakeGenericType(t);
                    var fakeObjectProperty = parameterFakeType.GetProperty("FakedObject", t, new Type[0]);
                    var fakeInstance = Activator.CreateInstance(parameterFakeType);

                    if (fakeObjectProperty == null)
                    {
                        throw new Exception($"Could not create a FakeObject for type {t.FullName}");
                    }

                    var objectInstance = fakeObjectProperty.GetValue(fakeInstance, null);
                    _fakeRepository.RegisterFake(fakeInstance, objectInstance);

                    _containerBuilder.RegisterInstance(objectInstance).As(t).ExternallyOwned();
                }
                else
                {
                    _containerBuilder.RegisterInstance(Activator.CreateInstance(t)).As(t).ExternallyOwned();
                }
            }

            _containerBuilder.RegisterType<T>();
        }

        private static ConstructorInfo GetInjectionConstructor()
        {
            var constructors = typeof(T).GetConstructors();
            return constructors.FirstOrDefault();

            // To use with constructor attributes
            // var injectionConstructor = constructors.FirstOrDefault(x => x.GetCustomAttributes(false).Any(a => a is InjectionConstructorAttribute));
        }

        public Fake<T1> GetFake<T1>() where T1 : class
        {
            var fakeObject = _container.Resolve<T1>();
            return _fakeRepository.GetFake(fakeObject);
        }

        public T GenerateClass(params IConstructorNamedParameter[] parameters)
        {
            return _container.Resolve<T>(parameters.Select(p => new NamedParameter(p.Name, p.Value)));
        }
    }
}