using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SearchTree
{
	public class SearchTree
	{
		private TreeNode head;
				
		public SearchTree ()
		{
			head = null;		
		}
		
		public bool Insert(int Data){
			TreeNode node = new TreeNode(Data);
			lock(this){
				if(head == null){
					head = node;
					return true;
				}
			}
			lock(head){
				return InsertRecursive(node, head);
			}
		}
		

		public bool Remove(int key){
			if(head == null) return false;
			else {
				lock(this){
					if(head.Data == key){
						var auxRoot = new TreeNode(0);
						auxRoot.Left = head;
						var result = head.Remove(key, auxRoot);
						head = auxRoot.Left;
						return result;
					}
				}
				return head.Remove(key, null);
			}
		}

		public bool Contains(int key){
			return Contains(key, head);
		}
		
		private bool Contains(int key, TreeNode node){
			if(node == null) return false;
			if(node.Data == key) return true;
			if(key < node.Data) return Contains(key, node.Left);
			if(key > node.Data) return Contains (key, node.Right);
			return false;
		}

		//on method entry, the mutex for SubTreeNode is aquired by the current thread. There is no need to lock
		// Data node since it is thread-local (was created in public function, passed in as simple parameter)
		private bool InsertRecursive(TreeNode DataNode, TreeNode SubTreeRoot){
			var LeftValid = SubTreeRoot.Left != null;
			var RightValid = SubTreeRoot.Right != null;
			var DataDiff = DataNode.Data - SubTreeRoot.Data;

			if(DataDiff < 0){
				//goes to the left

					if(LeftValid)
						lock(SubTreeRoot.Left){
							return InsertRecursive(DataNode, SubTreeRoot.Left);
						}
					else{
						SubTreeRoot.Left = DataNode;
						return true;
					}
			}else if(DataDiff > 0){
				
					if(RightValid)
						lock(SubTreeRoot.Right){
							return InsertRecursive(DataNode, SubTreeRoot.Right);
						}
					else{
						SubTreeRoot.Right = DataNode;
						return true;
					}
			}else{
				return false; //in the tree
			}
		}
		
		public IEnumerable<int> InOrder(){
			foreach(var x in InOrder (head))
				yield return x;
		}
		
		private IEnumerable<int> InOrder(TreeNode head){
			if(head != null){
				foreach(var x in InOrder(head.Left))
					yield return x;
				yield return head.Data;
				foreach(var x in InOrder(head.Right))
					yield return x;
			}
		}			
	}
	class TreeNode {
		public int Data;
		public TreeNode Left, Right;

		public TreeNode( int _Data){
			Data = _Data;
		}

		public bool Remove(int value, TreeNode parent){
			lock(this){
				if(value < this.Data){
					if(Left != null){
						var val = Left.Remove(value, this);
						return val;
					}else{
						var val = Right.Remove(value, this);
						return val;
					}
				}else if(value > this.Data){
					if(Right != null){
						var val = Right.Remove(value, this);
						return val;
					}else{ 
						return false;
					}
				}else{
					if(Left != null && Right != null){
						this.Data = Right.FindMin().Data;
						Right.Remove(this.Data, this);
					}else if(parent.Left == this){
						parent.Left = (Left == null) ? Right : Left;
					}else if(parent.Right == this){
						parent.Right = (Left == null) ? Right : Left;
					}
					return true;
				} // end mutex region
			}
		}
		private TreeNode FindMin(){
			if(this.Left == null) return this;
			else return Left.FindMin();
		}
		private TreeNode FindMax(){
			if(this.Right == null) return this;
			else return Right.FindMax();
		}
	}
}

