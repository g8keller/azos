﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Azos.Apps;
using Azos.Scripting;
using Azos.IO.Archiving;
using Azos.Data;
using Azos.Log;
using System.Threading.Tasks;

namespace Azos.Tests.Nub.IO.Archiving
{
  [Runnable]
  public class DefaultVolumeTests
  {
    [Run]
    public void Metadata_Create_Multiple_Sections_Mount()
    {
      //var ms = new FileStream("c:\\azos\\archive.lar", FileMode.Create);//  new MemoryStream();
      var ms = new MemoryStream();

      var meta = VolumeMetadataBuilder.Make("20210115-1745-doctor")
                                      .SetVersion(99, 21)
                                      .SetDescription("B vs D De-Terminator for doctor bubblegumization")
                                      .SetChannel(Atom.Encode("dvop"))
                                      .SetCompressionScheme(DefaultVolume.COMPRESSION_SCHEME_GZIP_MAX)
                                      .SetEncryptionScheme("marsha-keys2")
                                      .SetApplicationSection(app => {
                                        app.AddChildNode("user").AddAttributeNode("id", 111222);
                                        app.AddChildNode("user").AddAttributeNode("id", 783945);
                                        app.AddAttributeNode("is-good", false);
                                        app.AddAttributeNode("king-signature", Platform.RandomGenerator.Instance.NextRandomWebSafeString(150, 150));
                                      })
                                      .SetApplicationSection(app => { app.AddAttributeNode("a", false); })
                                      .SetApplicationSection(app => { app.AddAttributeNode("b", true); })
                                      .SetCompressionSection(cmp => { cmp.AddAttributeNode("z", 41); })
                                      .SetEncryptionSection(enc => { enc.AddAttributeNode("z", 99); });

      var volume = new DefaultVolume(NOPApplication.Instance.SecurityManager.Cryptography, meta, ms);

      volume.Dispose();
     // ms.GetBuffer().ToDumpString(DumpFormat.Hex).See();

      volume = new DefaultVolume(NOPApplication.Instance.SecurityManager.Cryptography, ms);
      volume.Metadata.See();
    }


    [Run]
    public void Write_LogMessages()
    {
      var msData = new FileStream("c:\\azos\\logging.lar", FileMode.Create);//  new MemoryStream();
      var msIdxId = new FileStream("c:\\azos\\logging.guid.lix", FileMode.Create);

      var meta = VolumeMetadataBuilder.Make("log messages")
                                      .SetVersion(1, 1)
                                      .SetDescription("Testing")
                                      .SetChannel(Atom.Encode("tezt"));

      var volumeData = new DefaultVolume(NOPApplication.Instance.SecurityManager.Cryptography, meta, msData);
      var volumeIdxId = new DefaultVolume(NOPApplication.Instance.SecurityManager.Cryptography, meta, msIdxId);


      volumeData.PageSizeBytes = 512*1024;
      volumeIdxId.PageSizeBytes = 512*1024;

      const int CNT = 16_000_000;
      var time = Azos.Time.Timeter.StartNew();


      Parallel.For(0, 16, _ =>{

        var aIdxId = new GuidIdxAppender(volumeIdxId,
                                          NOPApplication.Instance.TimeSource,
                                          NOPApplication.Instance.AppId, "dima@zhaba");

        var appender = new LogMessageArchiveAppender(volumeData,
                                               NOPApplication.Instance.TimeSource,
                                               NOPApplication.Instance.AppId,
                                               "dima@zhaba",
                                               onPageCommit: (e, b) => aIdxId.Append(new GuidBookmark(e.Guid, b)));


        for (var i = 0; i < CNT / 16; i++)
        {
          var msg = new Message()
          {
            Source = 1,
            Type = MessageType.DebugC,
            From = "method1",
            Topic = "Testing how",
            Text = "it is a b vs d determination for you",
            Exception = (i & 0xf) == 0 ? new Exception("This is an error") : null,
              RelatedTo = Guid.NewGuid()
          }.InitDefaultFields(NOPApplication.Instance);

          appender.Append(msg);
        }


        appender.Dispose();
        aIdxId.Dispose();

      });

      time.Stop();
      "Did {0} in {1:n1} sec at {2:n2} ops/sec".SeeArgs(CNT, time.ElapsedSec, CNT / time.ElapsedSec);

      volumeIdxId.Dispose();
      volumeData.Dispose();

      "CLOSED all".See();
    }



    public class PhoneCall : TypedDoc
    {
      [Field] public Guid Id     { get; set; }
      [Field] public DateTime StartUtc { get; set; }
      [Field] public DateTime EndUtc { get; set; }
      [Field] public string From { get; set; }
      [Field] public string To { get; set; }

      public static PhoneCall NewFake()
        => new PhoneCall
        {
          Id = Guid.NewGuid(),
          StartUtc = DateTime.UtcNow,
          EndUtc = DateTime.UtcNow.AddMinutes(5),
          From = Azos.Text.NaturalTextGenerator.GenerateFullName(),
          To = Azos.Text.NaturalTextGenerator.GenerateFullName()
        };
    }


    [Run]
    public void Write_PhoneCalls()
    {
      var msData = new FileStream("c:\\azos\\calls.lar", FileMode.Create);//  new MemoryStream();
      var msIdxId = new FileStream("c:\\azos\\calls.id.lix", FileMode.Create);//  new MemoryStream();
      //                                                                        // var msIdxFrom = new FileStream("c:\\azos\\calls.from.lix", FileMode.Create);//  new MemoryStream();
      //                                                                        //var msData = new MemoryStream();
      //                                                                        //var msIdx1 = new MemoryStream();

      var meta = VolumeMetadataBuilder.Make("kikimor")
                                      .SetVersion(1, 1)
                                      .SetDescription("Testing")
                                      .SetChannel(Atom.Encode("tezt"));

      var volumeData = new DefaultVolume(NOPApplication.Instance.SecurityManager.Cryptography, meta, msData);
      var volumeIdxId = new DefaultVolume(NOPApplication.Instance.SecurityManager.Cryptography, meta, msIdxId);

      var aIdxId = new GuidIdxAppender(volumeIdxId,
                                        NOPApplication.Instance.TimeSource,
                                        NOPApplication.Instance.AppId, "dima@zhaba");

      var appender = new JsonArchiveAppender(volumeData,
                                             NOPApplication.Instance.TimeSource,
                                             NOPApplication.Instance.AppId,
                                             "dima@zhaba",
                                             onPageCommit: (e, b) => aIdxId.Append(new GuidBookmark(((PhoneCall)e).Id, b)));

      volumeData.PageSizeBytes = 8024;
      volumeIdxId.PageSizeBytes = 1024;
      for (var i = 0; i < 500_000; i++)
      {
        var call = PhoneCall.NewFake();
        appender.Append(call);
      }

      appender.Dispose();
      aIdxId.Dispose();

      volumeIdxId.Dispose();
      volumeData.Dispose();

      "CLOSED all".See();
    }
  }
}