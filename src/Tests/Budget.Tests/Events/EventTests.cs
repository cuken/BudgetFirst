﻿// BudgetFirst 
// ©2016 Thomas Mühlgrabner
//
// This source code is dual-licensed under:
//   * Mozilla Public License 2.0 (MPL 2.0) 
//   * GNU General Public License v3.0 (GPLv3)
//
// ==================== Mozilla Public License 2.0 ===================
// This Source Code Form is subject to the terms of the Mozilla Public 
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
// ================= GNU General Public License v3.0 =================
// This file is part of BudgetFirst.
//
// BudgetFirst is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BudgetFirst is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Budget First.  If not, see<http://www.gnu.org/licenses/>.
// ===================================================================

namespace Budget.Domain.Tests.Events
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using BudgetFirst.Events.Events;
    using BudgetFirst.Infrastructure.Messaging;
    using BudgetFirst.Infrastructure.Serialisation;

    using NUnit.Framework;

    /// <summary>
    /// Contains tests regarding domain events
    /// </summary>
    [TestFixture]
    public class EventTests
    {
        /// <summary>
        /// AccountCreated has all fields after a serialisation round trip
        /// </summary>
        [Test]
        public void AccountCreatedSerialisationRoundtripHasAllFields()
        {
            const string AccountName = "Account name";

            var accountCreated = new AccountCreated(AccountName);
            var roundtripped = this.SerialisationRoundtrip(accountCreated);

            Assert.AreNotSame(accountCreated, roundtripped);

            Assert.AreEqual(accountCreated.Name, roundtripped.Name);
            
            this.AssertDomainEventPropertiesAreEqual(accountCreated, roundtripped);
        }

        /// <summary>
        /// AccountCreated has all fields after a serialisation round trip
        /// </summary>
        [Test]
        public void AccountNameChangedSerialisationRoundtripHasAllFields()
        {
            const string AccountName = "Account name";

            var accountNameChanged = new AccountNameChanged(AccountName);
            var roundtripped = this.SerialisationRoundtrip(accountNameChanged);

            Assert.AreNotSame(accountNameChanged, roundtripped);

            Assert.AreEqual(accountNameChanged.Name, roundtripped.Name);

            this.AssertDomainEventPropertiesAreEqual(accountNameChanged, roundtripped);
        }

        /// <summary>
        /// Assert that all properties of the domain event are equal
        /// </summary>
        /// <param name="source">Source before roundtrip</param>
        /// <param name="actual">Event after roundtrip</param>
        private void AssertDomainEventPropertiesAreEqual(DomainEvent source, DomainEvent actual)
        {
            Assert.AreEqual(source.VectorClock, actual.VectorClock);
            Assert.AreEqual(source.AggregateId, actual.AggregateId);
            Assert.AreEqual(source.DeviceId, actual.DeviceId);
            Assert.AreEqual(source.EventId, actual.EventId);
            Assert.AreEqual(source.Timestamp, actual.Timestamp);
        }

        /// <summary>
        /// Perform a serialisation roundtrip
        /// </summary>
        /// <param name="source">Source object to serialise and de-serialise</param>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <returns>Clone of source object through serialisation roundtrip</returns>
        private TEvent SerialisationRoundtrip<TEvent>(TEvent source) where TEvent : DomainEvent
        {
            string serialisedObject;

            using (var memoryStream = new MemoryStream())
            {
                Serialiser.Serialise(source, memoryStream);
                memoryStream.Position = 0; // rewind
                serialisedObject = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            TEvent roundtrip;

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(serialisedObject);
                    streamWriter.Flush();

                    // do not close stream yet
                    memoryStream.Position = 0; // rewind

                    roundtrip = Serialiser.DeSerialise<TEvent>(memoryStream);
                }
            }

            return roundtrip;
        }
    }
}
