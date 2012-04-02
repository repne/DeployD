/* JSINQ, Copyright (c) 2010 Kai J�ger, governed by an MIT-style license. */ 
if(typeof jsinq=="undefined"){jsinq={}}(function(){function m(){}m.prototype=new Error();m.prototype.name="InvalidOperationException";m.prototype.message="Operation is not valid due to the current state of the object.";function f(n){this.message+=n}f.prototype=new Error();f.prototype.name="ArgumentException";f.prototype.message="Value provided for the following parameter is invalid: ";function h(n){this.message+=n}h.prototype=new Error();h.prototype.name="ArgumentOutOfRangeException";h.prototype.message="Specified argument was out of the range of valid values.\r\nParameter name: ";function c(){}c.prototype=new Error();c.prototype.name="KeyNotFoundException";c.prototype.message="Key does not exist";function g(){}g.prototype=new Error();g.prototype.name="NotSupportedException";g.prototype.message="Method is not supported";function k(){}k.prototype={equals:function(o,n){return o==n}};(function(){var n=new k();k.getDefault=function(){return n};k.fromFunction=function(p){var o=new k();o.equals=p;return o}})();function b(){}b.prototype={compare:function(o,n){return o<n?-1:(o>n?1:0)}};(function(){var n=new b();b.getDefault=function(){return n};b.fromFunction=function(p){var o=new b();o.compare=p;return o}})();function a(n){if(arguments.length==0){n=[]}else{if(typeof n.length=="undefined"||(n.length>0&&typeof n[0]=="undefined")){n=[n]}}this.getEnumerator=function(){return new function(){var o=-1;this.moveNext=function(){++o;return o<n.length};this.current=function(){if(o<0||o>=n.length){throw new m()}return n[o]};this.reset=function(){o=-1}}}}a.empty=function(){return new a()};a.range=function(p,o){if(o<0){throw new h()}var n=function(){this.getEnumerator=function(){return new function(){var r=-1;var q=false;this.moveNext=function(){q=false;if(r<o-1){q=true;++r;return true}return false};this.current=function(){if(q){return p+r}else{throw new m()}};this.reset=function(){r=-1}}}};n.prototype=a.prototype;return new n()};a.repeat=function(n,p){if(p<0){throw new h()}var o=function(){this.getEnumerator=function(){return new function(){var r=-1;var q=false;this.moveNext=function(){q=false;if(r<p-1){q=true;++r;return true}return false};this.current=function(){if(q){return n}else{throw new m()}};this.reset=function(){r=-1;q=false}}}};o.prototype=a.prototype;return new o()};a.prototype=(function(){function o(s){return s}var p=1;var n=-1;function r(s){return function(u){var y=this;var x=[];var v=b.getDefault();if(arguments.length>=3){v=arguments[1];if(typeof v=="function"){v=b.fromFunction(v)}x=arguments[2]}else{if(arguments.length>=2){if(arguments[1] instanceof Array){x=arguments[1]}else{v=arguments[1];if(typeof v=="function"){v=b.fromFunction(v)}}}}var t=[[u,s,v]];t=t.concat(x);var w=function(){this.getEnumerator=function(){return new function(){var A=null;function z(){var B=y.toArray();B.sort(function(K,I){var L=0;var F;var J;var H;for(var G=t.length-1;G>=0;G--){L=0;F=t[G][0];J=t[G][1];H=t[G][2];var E=F(K);var D=F(I);var C=H.compare(E,D);if(C!=0){return C*J}}return 0});A=(new a(B)).getEnumerator()}this.moveNext=function(){if(A==null){z()}return A.moveNext()};this.current=function(){if(A==null){throw new m()}return A.current()};this.reset=function(){if(A!=null){A.reset()}}}};this.thenBy=function(A,B){var z=r(p);if(arguments.length<2){B=b.getDefault()}if(typeof B=="function"){B=b.fromFunction(B)}return z.call(y,A,B,t)};this.thenByDescending=function(A,B){var z=r(n);if(arguments.length<2){B=b.getDefault()}if(typeof B=="function"){B=b.fromFunction(B)}return z.call(y,A,B,t)}};w.prototype=a.prototype;return new w()}}function q(s){return function(v,t,y,u,w){var z=this;if(arguments.length<5){w=k.getDefault()}if(typeof w=="function"){w=k.fromFunction(w)}var x=function(){this.getEnumerator=function(){return new function(){var H=null;var G=z.getEnumerator();var A=false;var D;var B;var F=null;var C=-1;function E(){H=new e(w);var I=v.getEnumerator();while(I.moveNext()){var K=I.current();var J=y(K);if(!H.tryAdd(J,[K])){var L=H.item(J);L.push(K)}}}this.moveNext=function(){if(H==null){E()}var J;var I;if(s){A=G.moveNext();if(A){J=G.current();I=t(J);D=J;if(H.containsKey(I)){B=new a(H.item(I))}else{B=a.empty()}}}else{if(F!=null&&++C<F.length){B=F[C];A=true}else{A=false;while(G.moveNext()){J=G.current();I=t(J);if(H.containsKey(I)){F=H.item(I);B=F[0];C=0;A=true;D=J;break}else{continue}}}}return A};this.current=function(){if(!A){throw new m()}return u(D,B)};this.reset=function(){G.reset()}}}};x.prototype=a.prototype;return new x()}}return{aggregate:function(){var v=this.getEnumerator();var s;var u;var t=o;if(arguments.length>=2){s=arguments[0];u=arguments[1];if(arguments.length>=3){t=arguments[2]}}else{if(!v.moveNext()){throw new m()}u=arguments[0];s=v.current()}while(v.moveNext()){s=u(s,v.current())}return t(s)},all:function(s){var t=this.getEnumerator();while(t.moveNext()){if(!s(t.current())){return false}}return true},any:function(s){var u=this.getEnumerator();if(arguments.length==0){return u.moveNext()}var t=[];while(u.moveNext()){if(s(u.current())){return true}}return false},average:function(s){if(arguments.length==0){s=o}var u=1;var t=this.aggregate(function(v,w){++u;return v+s(w)});return t/u},concat:function(s){var u=this;var t=function(){this.getEnumerator=function(){return new function(){var x=u.getEnumerator();var v=s.getEnumerator();var y=x;var w=true;this.moveNext=function(){if(!y.moveNext()){if(w){y=v;return y.moveNext()}return false}return true};this.current=function(){return y.current()};this.reset=function(){w=true;x.reset();v.reset();y=x}}}};t.prototype=a.prototype;return new t()},contains:function(t,s){if(arguments.length==1){s=k.getDefault()}if(typeof s=="function"){s=k.fromFunction(s)}return this.any(function(u){return s.equals(u,t)})},count:function(s){var t=0;var v=this.getEnumerator();var u=typeof s=="function";while(v.moveNext()){if((u&&s(v.current()))||!u){++t}}return t},defaultIfEmpty:function(s){var t=!this.any(function(u){return true});if(t){return new a(s)}return this},distinct:function(t){var v=this;var s=arguments.length>0;if(typeof t=="function"){t=k.fromFunction(t)}var u=function(){this.getEnumerator=function(){return new function(){var x=v.getEnumerator();var w;if(s){w=new e(t)}else{w=new e()}this.moveNext=function(){while(x.moveNext()){if(w.tryAdd(x.current(),true)){return true}}return false};this.current=function(){return x.current()};this.reset=function(){x.reset();w.clear()}}}};u.prototype=a.prototype;return new u()},elementAt:function(s){var t;var u=this.any(function(v){t=v;return s--==0});if(!u){throw new h("index")}return t},elementAtOrDefault:function(t,s){try{return this.elementAt(t)}catch(u){return s}},except:function(s,t){if(arguments.length<2){t=k.getDefault()}if(typeof t=="function"){t=k.fromFunction(t)}return this.where(function(u){return !s.any(function(v){return t.equals(u,v)})})},first:function(s){if(arguments.length==0){try{return this.elementAt(0)}catch(v){throw new m()}}else{var t;var u=this.any(function(w){if(s(w)){t=w;return true}return false});if(!u){throw new m()}return t}},firstOrDefault:function(){if(arguments.length==1){var t=arguments[0]}else{if(arguments.length>1){var s=arguments[0];var t=arguments[1]}}try{if(arguments.length>1){return this.first(s)}else{return this.first()}}catch(u){return t}},groupBy:function(){var t=arguments[0];var w=o;var s=o;var u=k.getDefault();if(arguments.length==2){if(typeof arguments[1].equals=="function"){u=arguments[1]}else{if(arguments[1].arity>=2){s=arguments[1]}else{w=arguments[1]}}}else{if(arguments.length==3){if(arguments[1].arity>=2){s=arguments[1];u=arguments[2]}else{w=arguments[1];if(typeof arguments[2].equals=="function"){u=arguments[2]}else{s=arguments[2]}}}else{if(arguments.length>3){w=arguments[1];s=arguments[2];u=arguments[3]}}}if(typeof u=="function"){u=k.fromFunction(u)}var x=this;var v=function(){this.getEnumerator=function(){return new function(){var z=null;var y=-1;function A(){var D=x.getEnumerator();var F=new e(u);while(D.moveNext()){var C=D.current();var B=t(C);if(F.containsKey(B)){var E=F.item(B);E.push(w(C))}else{F.tryAdd(B,[w(C)])}}z=F.toArray()}this.moveNext=function(){if(z==null){A()}++y;return y<z.length};this.current=function(){if(y<0||y>=z.length){throw new m()}var C=z[y];var B=new a(C.value);if(s!=o){return s(C.key,B)}else{return new j(C.key,B)}};this.reset=function(){y=-1}}}};v.prototype=a.prototype;return new v()},groupJoin:q(true),intersect:function(s,t){if(arguments.length<2){t=k.getDefault()}if(typeof t=="function"){t=k.fromFunction(t)}return this.distinct(t).where(function(u){return s.contains(u,t)})},join:q(false),last:function(s){var u=typeof s=="function";var t;var v=true;this.any(function(w){if(!u||s(w)){t=w;v=false}return false});if(v){throw new m()}return t},lastOrDefault:function(){if(arguments.length==1){var t=arguments[0]}else{if(arguments.length>1){var s=arguments[0];var t=arguments[1]}}try{if(arguments.length>1){return this.last(s)}else{return this.last()}}catch(u){return t}},max:function(t){if(arguments.length==0){t=o}var s=true;return this.aggregate(function(u,v){if(s){u=t(u);s=false}return Math.max(u,t(v))})},min:function(t){if(arguments.length==0){t=o}var s=true;return this.aggregate(function(u,v){if(s){u=t(u);s=false}return Math.min(u,t(v))})},orderBy:r(p),orderByDescending:r(n),reverse:function(){var t=this;var s=function(){this.getEnumerator=function(){return new function(){var u=null;this.moveNext=function(){if(u==null){u=new a(t.toArray().reverse()).getEnumerator()}return u.moveNext()};this.current=function(){if(u==null){throw new m()}return u.current()};this.reset=function(){if(u!=null){u.reset()}}}}};s.prototype=a.prototype;return new s()},select:function(s){var u=this;var t=function(){this.getEnumerator=function(){return new function(){var w=u.getEnumerator();var v=-1;this.moveNext=function(){++v;return w.moveNext()};this.current=function(){return s(w.current(),v)};this.reset=function(){w.reset();v=-1}}}};t.prototype=a.prototype;return new t()},selectMany:function(s,t){var v=arguments.length>1;var w=this;var u=function(){this.getEnumerator=function(){return new function(){var B=w.getEnumerator();var A=null;var x=null;var z=false;var y=0;this.moveNext=function(){z=false;var C=true;while(true){if(x==null||!x.moveNext()){if(!B.moveNext()){break}A=B.current();x=s(A,y++).getEnumerator();C=false}if(C||x.moveNext()){z=true;break}}return z};this.current=function(){if(!z){throw new m()}if(v){return t(A,x.current())}else{return x.current()}};this.reset=function(){z=false;x=null;A=null;B.reset();y=0}}}};u.prototype=a.prototype;return new u()},sequenceEqual:function(t,u){if(this==t){return true}if(arguments.length<2){u=k.getDefault()}if(typeof u=="function"){u=k.fromFunction(u)}var v=this.getEnumerator();var s=t.getEnumerator();while(v.moveNext()&&s.moveNext()){if(!u.equals(v.current(),s.current())){return false}}return true},single:function(s){var u=arguments.length>0;var v=this.getEnumerator();if(!v.moveNext()){throw new m()}var t=v.current();if(v.moveNext()||(u&&!s(t))){throw new m()}return t},singleOrDefault:function(){if(arguments.length==1){var t=arguments[0]}else{if(arguments.length>1){var s=arguments[0];var t=arguments[1]}}try{if(arguments.length>1){return this.single(s)}else{return this.single()}}catch(u){return t}},skip:function(s){if(s==0){return this}return this.skipWhile(function(u,t){return t<s})},skipWhile:function(s){var u=this;var t=function(){this.getEnumerator=function(){return new function(){var x=u.getEnumerator();var w=false;var v=0;this.moveNext=function(){if(!w){var y=false;while((y=x.moveNext())&&s(x.current(),v++)){}w=true;return y}else{return x.moveNext()}};this.current=function(){return x.current()};this.reset=function(){x.reset();w=false;v=0}}}};t.prototype=a.prototype;return new t()},sum:function(s){if(arguments.length==0){s=o}return this.aggregate(0,function(t,u){return t+s(u)})},take:function(s){if(s==0){return a.empty()}return this.takeWhile(function(u,t){return t<s})},takeWhile:function(s){var u=this;var t=function(){this.getEnumerator=function(){return new function(){var x=u.getEnumerator();var w=true;var v=0;this.moveNext=function(){if(w){w=x.moveNext()&&s(x.current(),v++)}return w};this.current=function(){if(!w){throw new m()}return x.current()};this.reset=function(){x.reset();w=true;v=0}}}};t.prototype=a.prototype;return new t()},toArray:function(){var t=this.getEnumerator();var s=[];while(t.moveNext()){s.push(t.current())}return s},union:function(s,t){if(arguments.length<2){t=k.getDefault()}if(typeof t=="function"){t=k.fromFunction(t)}return this.concat(s).distinct(t)},where:function(s){var u=this;var t=function(){this.getEnumerator=function(){return new function(){var x=u.getEnumerator();var w;var v=false;this.moveNext=function(){v=false;while(x.moveNext()){w=x.current();if(s(w)){v=true;break}}return v};this.current=function(){if(!v){throw new m()}return w};this.reset=function(){x.reset();v=false}}}};t.prototype=a.prototype;return new t()},zip:function(t,s){var v=this;var u=function(){this.getEnumerator=function(){return new function(){var y=v.getEnumerator();var w=t.getEnumerator();var x=false;this.moveNext=function(){if(y.moveNext()&&w.moveNext()){x=true;return true}else{return false}};this.current=function(){if(!x){throw new m()}return s(y.current(),w.current())};this.reset=function(){y.reset();w.reset()}}}};u.prototype=a.prototype;return new u()},toDictionary:function(t,u){if(typeof u=="function"){u=k.fromFunction(u)}var x=this.getEnumerator();var w;if(arguments.length==1){w=new e()}else{if(arguments.length>=2){w=new e(u)}}var s;var v;while(x.moveNext()){v=x.current();s=t(v);if(!w.tryAdd(s,v)){throw new f("keySelector")}}return w},toList:function(){return new l(this)},toLookup:function(z){var z=arguments[0];var s=null;var v=null;var u;switch(arguments.length){case 2:if(typeof arguments[1]=="function"){s=arguments[1]}else{v=arguments[1]}break;case 3:s=arguments[1];if(typeof arguments[2]=="function"){v=k.fromFunction(arguments[2])}else{v=arguments[2]}break}if(v!=null){u=new d(v)}else{u=new d()}var x=this.getEnumerator();var A;var y;var t;while(x.moveNext()){y=x.current();A=z(y);try{t=u.item(A)}catch(w){t=new j(A);u.add(t)}if(s){t.add(s(y))}else{t.add(y)}}return u},each:function(s){var t=this.getEnumerator();while(t.moveNext()){s(t.current())}}}})();function e(v){var n=v==k.getDefault();if(arguments.length>0&&!n){v=arguments[0]}else{v=null}if(typeof v=="function"){v=k.fromFunction(v)}var w=0;var y={};var t={};var r=8;var u=24;var p={}.toString();function o(A){if(v!=null){return 0}var z=A.toString().substring(0,u);var B=A.constructor||"";var D=[B.toString(),z];if(A.nodeName){D.push(A.nodeName)}if(A.parentNode&&A.parentNode.nodeName){D.push(A.parentNode.nodeName)}if(z==p){for(var C in A){D.push(C);D.push(A[C].toString().substring(0,u));if(D.length>u){break}}}return D.join("")}function q(z){return typeof z=="string"||typeof z=="number"||typeof key=="boolean"||z===null}function s(H,A){var F;if(v==null&&q(H)){var G=y[H];if(G instanceof Array){F=A(G[0])}else{F=A()}if(F.length>1){if(typeof G=="undefined"){++w}y[H]=[F[1]]}return F[0]}else{var z=o(H);var D=t[z];if(D instanceof Array){var I;var B=D.length;for(var C=0;C<B;C++){I=D[C];if((v!=null&&v.equals(I.key,H))||(v==null&&I.key==H)){F=A(I.value);if(F.length>1){I.value=F[1]}return F[0]}}}F=A();if(F.length>1){var E={key:H,value:F[1]};if(D instanceof Array){D.push(E)}else{t[z]=[E]}++w}return F[0]}}function x(A,B,z){return s(A,function(C){if(arguments.length==0||z){return[true,B]}else{return[false]}})}this.item=function(z){return s(z,function(A){if(arguments.length==0){throw new c(z)}return[A]})};this.set=function(z,A){x(z,A,true)};this.count=function(){return w};this.getComparer=function(){if(n){return k.getDefault()}else{return v}};this.add=function(z,A){if(!x(z,A,false)){throw new f("key")}};this.tryAdd=function(z,A){return x(z,A,false)};this.keys=function(){var A=[];var B=this.toArray();for(var z=0;z<B.length;z++){A.push(B[z].key)}return new a(A)};this.values=function(){var z=[];var B=this.toArray();for(var A=0;A<B.length;A++){z.push(B[A].value)}return new a(z)};this.clear=function(){w=0;y={};t={}};this.containsKey=function(z){return s(z,function(A){return[arguments.length==1]})};this.containsValue=function(B){var A=this.toArray();for(var z=0;z<A.length;z++){if(A[z].value==B){return true}}return false};this.getEnumerator=function(){return new a(this.toArray()).getEnumerator()};this.toArray=function(){var z=[];for(var B in y){z.push({key:B,value:y[B][0]})}var C;var A;for(B in t){C=t[B];for(A=0;A<C.length;A++){z.push(C[A])}}return z};this.remove=function(A){if(q(A)&&y[A] instanceof Array){delete y[A];--w;return true}else{var C=o(A);var B=t[C];if(B instanceof Array){for(var z=0;z<B.length;z++){if((v!=null&&v.equals(B[z].key,A))||(v==null&&B[z].key==A)){if(B.length>1){B.splice(z,1)}else{delete t[C]}--w;return true}}}return false}};this.isReadOnly=function(){return false};this.toString=function(){var z=[];this.each(function(A){z.push(A.key.toString()+":"+A.value.toString())});return z.join(", ")}}e.prototype=a.prototype;var l=function(p){var o;var n;if(typeof p!="undefined"){o=p.toArray()}else{o=[]}n=new a(o);this.add=function(q){o.push(q)};this.item=function(q){if(q<0||q>o.length-1){throw new h("index")}return o[q]};this.set=function(q,r){if(q<0||q>o.length-1){throw new h("index")}o[q]=r};this.addRange=function(q){var r=q.getEnumerator();while(r.moveNext()){o.push(r.current())}};this.asReadOnly=function(){return new i(this)};this.binarySearch=function(){var v;var t=b.getDefault();var r=0;var u=o.length;switch(arguments.length){case 1:v=arguments[0];break;case 2:v=arguments[0];t=arguments[1];break;case 4:r=arguments[0];u=arguments[1];v=arguments[2];t=arguments[3];break}if(typeof t=="function"){t=b.fromFunction(t)}if(u==0){return ~u}else{if(u==1){if(t.compare(v,o[0])==0){return 0}else{return ~u}}}var x=r;var s=r+u-1;var q;var w;while(x<=s){q=parseInt((x+s)/2);w=t.compare(v,o[q]);if(w>0){x=q+1}else{if(w<0){s=q-1}else{return q}}}return ~x};this.clear=function(){o.length=0};this.copyTo=function(){var u;var q=0;var r=0;var t=o.length;switch(arguments.length){case 1:u=arguments[0];break;case 2:u=arguments[0];q=arguments[1];break;case 4:r=arguments[0];u=arguments[1];q=arguments[2];t=r+arguments[3];break}if(r<0||q<0||t<0){throw new h()}else{if(t>0&&t>o.length){throw new f()}}for(var s=r;s<t;s++,q++){u[q]=o[s]}};this.exists=function(q){for(var r=0;r<o.length;r++){if(q(o[r])){return true}}return false};this.find=function(r){var q=this.findIndex(r);if(q>=0){return o[q]}else{return null}};this.findAll=function(q){var s=new l();for(var r=0;r<o.length;r++){if(q(o[r])){s.add(o[r])}}return s};this.findIndex=function(){var q=null;var u=0;var t=0;var s=o.length;switch(arguments.length){case 1:q=arguments[0];break;case 2:u=arguments[0];s=o.length-u;q=arguments[1];break;case 3:u=arguments[0];s=arguments[1];q=arguments[2];break}if(u<0||(o.length>0&&u>=o.length)||s<0||u+s<0||u+s>o.length){throw new h()}t=Math.min(o.length,u+s);for(var r=u;r<t;r++){if(q(o[r])){return r}}return -1};this.findLast=function(r){var q=this.findLastIndex(r);if(q>=0){return o[q]}else{return null}};this.findLastIndex=function(){var q=null;var u=0;var t=0;var s=o.length;switch(arguments.length){case 1:q=arguments[0];break;case 2:u=arguments[0];s=o.length-u;q=arguments[1];break;case 3:u=arguments[0];s=arguments[1];q=arguments[2];break}if(u<0||(o.length>0&&u>=o.length)||s<0||u+s<0||u+s>o.length){throw new h()}t=Math.min(o.length,u+s);for(var r=t-1;r>=u;r--){if(q(o[r])){return r}}return -1};this.forEach=function(r){for(var q=0;q<o.length;q++){r(o[q])}};this.getEnumerator=function(){return n.getEnumerator()};this.getRange=function(r,t){if(r<0||r>=o.length){throw new h("index")}if(r+t>o.length){throw new h("count")}var q=new l();var u=r+t;for(var s=r;s<u;s++){q.add(o[s])}return q};this.indexOf=function(t,r,s){var q=function(u){return u==t};switch(arguments.length){case 1:return this.findIndex(q);case 2:return this.findIndex(r,o.length-r,q);case 3:return this.findIndex(r,s,q)}};this.insert=function(q,r){if(q<0||q>o.length){throw new h("index")}o.splice(q,0,r)};this.insertRange=function(q,s){if(q<0||q>o.length){throw new h("index")}var t=s.getEnumerator();var r=0;while(t.moveNext()){o.splice(q+r,0,t.current());++r}};this.lastIndexOf=function(t,r,s){var q=function(u){return u==t};switch(arguments.length){case 1:return this.findLastIndex(q);case 2:return this.findLastIndex(r,o.length-r,q);case 3:return this.findLastIndex(r,s,q)}};this.remove=function(r){var q=this.indexOf(r);if(q>-1){o.splice(q,1);return true}else{return false}};this.removeAll=function(r){var q=[];for(var s=0;s<o.length;s++){if(!r(o[s])){q.push(o[s])}}var t=o.length-q.length;o=q;n=new a(o);return t};this.removeAt=function(q){if(q<0||q>=o.length){throw new h("index")}o.splice(q,1)};this.removeRange=function(q,r){if(q<0||q>=o.length){throw new h("index")}if(q+r>o.length){throw new h("count")}o.splice(q,r)};this.reverse=function(){var q=0;var s=o.length;if(arguments.length==2){q=arguments[0];s=arguments[1];var t=o.slice(q,q+s).reverse();for(var r=0;r<t.length;r++){o.splice(q+r,1,t[r])}}else{o=o.reverse()}};this.sort=function(){var u=null;var s=0;var v=o.length;var q=function(x,w){if(u){if(typeof u=="function"){return u(x,w)}else{return u.compare(x,w)}}else{if(w>x){return 1}else{if(w==x){return 0}else{return -1}}}};if(arguments.length==0){u=b.getDefault();o.sort(q)}else{if(arguments.length==1){u=arguments[0];o.sort(q)}else{if(arguments.length==3){s=arguments[0];v=arguments[1];u=arguments[2];var r=o.slice(s,s+v).sort(q);for(var t=0;t<r.length;t++){o.splice(s+t,1,r[t])}}}}};this.toArray=function(){var r=[];for(var q=0;q<o.length;q++){r.push(o[q])}return r};this.toString=function(){return this.toArray().join(", ")};this.isReadOnly=function(){return false};this.trueForAll=function(q){if(o.length==0){return false}for(var r=0;r<o.length;r++){if(!q(o[r])){return false}}return true}};l.prototype=a.prototype;var i=function(n){this.contains=function(o){return n.contains(o)};this.copyTo=function(p,o){return n.copyTo(p,o)};this.count=function(){return n.count()};this.getEnumerator=function(){return n.getEnumerator()};this.indexOf=function(o){return n.indexOf(o)};this.item=function(){return n.item()};this.toString=function(){return n.toString()};this.isReadOnly=function(){return true};this.add=this.clear=this.insert=this.remove=this.removeAt=function(){throw new g()}};i.prototype=a.prototype;var j=function(p,n){var o=[];if(!n){n=new a(o)}else{o=null}this.key=p;this.getKey=function(){return this.key};this.add=function(q){o.push(q)};this.getEnumerator=function(){return n.getEnumerator()}};j.prototype=a.prototype;var d=function(n){var o;if(n){o=new e(n)}else{o=new e()}this.add=function(p){o.add(p.key,p)};this.applyResultSelector=function(p){var r=this;var q=function(){this.getEnumerator=function(){return new function(){var s=r.getEnumerator();var t=null;var u;this.moveNext=function(){if(t==null||!t.moveNext()){if(s.moveNext()){u=s.current().key;t=s.current().getEnumerator();return t.moveNext()}else{t=null;return false}}return true};this.current=function(){if(t==null){throw new m()}return p(u,t.current())};this.reset=function(){s.reset();t=null}}}};q.prototype=a.prototype;return new q()};this.contains=function(p){return o.containsKey(p)};this.item=function(p){return o.item(p)};this.count=function(){return o.count()};this.getEnumerator=function(){return o.values().getEnumerator()}};d.prototype=a.prototype;this.InvalidOperationException=m;this.ArgumentException=f;this.ArgumentOutOfRangeException=h;this.KeyNotFoundException=c;this.NotSupportedException=g;this.EqualityComparer=k;this.Comparer=b;this.Enumerable=a;this.Dictionary=e;this.List=l;this.ReadOnlyCollection=i;this.Grouping=j;this.Lookup=d}).call(jsinq);