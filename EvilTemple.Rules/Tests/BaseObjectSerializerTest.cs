using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EvilTemple.Rules.Feats;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Ninject;
using NUnit.Framework;

namespace EvilTemple.Rules.Tests
{
    class BaseObjectSerializerTest
    {
        [Test]
        public void TestFeats()
        {

            var obj = new PlayerCharacter();
            obj.AddFeat(new FeatInstance("someFeat", "param1"));
            obj.AddFeat(new FeatInstance("someFeat2"));

            var writer = new StringWriter();

            BaseObjectSerializer.Serialize(writer, obj);

            Console.WriteLine(writer.ToString());

            var obj2 = BaseObjectSerializer.Deserialize(new JsonTextReader(new StringReader(writer.ToString())));
            Assert.IsAssignableFrom(typeof (PlayerCharacter), obj2);

            var pc = (PlayerCharacter) obj2;

            Assert.AreEqual(2, pc.Feats.Count);

            var fi1 = new FeatInstance("someFeat", "param1");
            Assert.AreEqual(fi1, pc.Feats[0]);

            var fi2 = new FeatInstance("someFeat2");
            Assert.AreEqual(fi2, pc.Feats[1]);

        }

        [Test]
        public void TestSkills()
        {

            var kernel = new StandardKernel();
            kernel.Bind<Skills>().ToSelf().InSingletonScope();
            Services.Kernel = kernel;
            
            var sk1 = new Skill {Id = "SomeSkill1"};
            var sk2 = new Skill {Id = "SomeSkill2"};
            kernel.Get<Skills>().Add(sk1);
            kernel.Get<Skills>().Add(sk2);

            var obj = new PlayerCharacter();
            obj.SetSkillRank(sk1, 15);
            obj.SetSkillRank(sk2, 10);

            var writer = new StringWriter();

            BaseObjectSerializer.Serialize(writer, obj);

            Console.WriteLine(writer.ToString());

            var obj2 = BaseObjectSerializer.Deserialize(new JsonTextReader(new StringReader(writer.ToString())));
            Assert.IsAssignableFrom(typeof(PlayerCharacter), obj2);

            var pc = (PlayerCharacter)obj2;

            Assert.AreEqual(15, pc.GetSkillRank(sk1));
            Assert.AreEqual(10, pc.GetSkillRank(sk2));
        }

        [Test]
        public void TestClassLevels()
        {
            var kernel = new StandardKernel();
            kernel.Bind<CharacterClasses>().ToSelf().InSingletonScope();
            Services.Kernel = kernel;

            var cc1 = new CharacterClass { Id = "warrior" };
            var cc2 = new CharacterClass { Id = "rogue" };
            kernel.Get<CharacterClasses>().Add(cc1);
            kernel.Get<CharacterClasses>().Add(cc2);

            var cl1 = new ClassLevel(cc1);
            cl1.Levels = 1;
            
            var cl2 = new ClassLevel(cc2);
            cl2.Levels = 5;

            var obj = new PlayerCharacter();
            obj.AddClassLevel(cl1);
            obj.AddClassLevel(cl2);

            var writer = new StringWriter();

            BaseObjectSerializer.Serialize(writer, obj);

            Console.WriteLine(writer.ToString());

            var obj2 = BaseObjectSerializer.Deserialize(new JsonTextReader(new StringReader(writer.ToString())));
            Assert.IsAssignableFrom(typeof(PlayerCharacter), obj2);

            var pc = (PlayerCharacter)obj2;

            var cl1Check = pc.GetClassLevel(cc1);
            Assert.IsNotNull(cl1Check);
            Assert.AreEqual(1, cl1Check.Levels);

            var cl2Check = pc.GetClassLevel(cc2);
            Assert.IsNotNull(cl2Check);
            Assert.AreEqual(5, cl2Check.Levels);
        }

    }
}
