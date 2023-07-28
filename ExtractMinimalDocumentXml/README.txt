
1. sdtを除くという処理

w:sdt/w:sdtContent/*までスキップする。

2. 表を調べる処理

見出しは残す
段落は削除する
表はattribute tableであれば削除
それ以外の表はキープ
表の中のartworkは削除

3, 図を調べる

見出しは残す
段落は削除する
図のある表はキープ
図のない表は削除



ロジックを書くのではなくXPathだけで動かす

見出しならキープする
w:doc/w:body/w:p
w:doc/w:body/w:pは消す
bookmarkstartとendは消す
attribute tableは消す
sdtの場合はスキップ