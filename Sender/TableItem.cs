using System;
using System.Collections.Generic;
using System.Text;

namespace Sender {
	class TableItem {
		public string name;
		public string database;
		public string identColumn;
		public int lastID;
		public string origin_table;
		public bool tgrTable;
		public int backoffTime;
		public int backoffCycle;

		public TableItem(string database, string name, string col, int id, string origin,
			bool tgrTable = false, int time = 0, int cycle = 2) {
			this.database = database;
			this.name = name;
			this.identColumn = col;
			this.lastID = id;
			this.origin_table = origin;
			this.tgrTable = tgrTable;
			this.backoffTime = time;
			this.backoffCycle = cycle;
		}
	}
}
