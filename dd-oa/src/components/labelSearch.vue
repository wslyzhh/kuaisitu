<!-- 标签搜索 -->
<template>
    <div class="labelSearch" v-show="show">
        <ul class="label_list">
            <li v-for="(item,index) in list" :key="index" :class="item.class">
                <h2 class="title" :class="item.class">{{item.title}}</h2>
                <div class="list flex flex_wrap" >
                    <span v-for="(label,l) in item.list" :class="{active:label.isChecked}" @click="activeLabel(l,index)">{{label.text}}</span>
                </div>
            </li>
        </ul>
        <div class="label_btn flex">
            <button class="reset" @click="chageReset">重置</button>
            <button class="confirm" @click="chageConfirm">确定</button>
        </div>
    </div>
</template>

<script>
export default {
    name:"",
    data() {
        return {
            /*
			list:[
			   {
			       title:'合同造价',
			       class:'greenTitle',
			       list:[]
			   },
			   {
			       title:'锁单状态',
			   	class:'blueTitle',
			       list:[]
			   },
			   {
			       title:'订单状态',
			   	class:'',
			       list:[]
			   },
			   {
			       title:'接单状态',
			   	class:'',
			       list:[]
			   },
		   ],
			*/
        }
    },
    props:{
        show:{
            type:Boolean,
            default:false
        },
		list:{
		    type:Array,
		    default:[]
		}
    },
    components: {},
    computed: {},
    mounted() {

    },
    methods: {
        activeLabel(_index,_pid){
			this.claerOneRowChecked(this.list[_pid].list)
            this.list[_pid].list[_index].isChecked = true;
        },
		claerOneRowChecked(_list){
			_list.map((item) => {
				item.isChecked = false;
			})
		},
		chageReset(){
		    this.list.map((item,index) => {
		        this.claerOneRowChecked(item.list)
				this.$set(item.list[0],'isChecked',true)
		    })
		},
        chageConfirm(){
			let actives = [];
			this.list.map((item,index) => {
				item.list.map((ll) => {
					if(ll.isChecked){
						actives.push({
							key:item.s_key,
							value:ll.value
						})
					}
				})
			})
            this.$emit('on-label',actives)
        }
    },
}
</script>

<style scoped lang='scss'>
    .labelSearch{
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        padding-bottom: .8rem;
		background-color: #FFF;
    }
    .label_list{
        padding: 0 .22rem;
        li{
            .title{
                position: relative;
                font-size: .32rem;
                color: #000;
                line-height: .9rem;
                padding-left: .3rem;
            }
            .title::before{
                content: '';
                position: absolute;
                left: 0;
                top: 50%;
                transform: translateY(-50%);
                width: .18rem;
                height: .18rem;
                border-radius: 50%;
                background-color:#f3b362;
            }
            .blueTitle::before{
                background-color: #65a7e1;
            }
            .greenTitle::before{
                background-color: #5dbd97;
            }
            .list{
                span{
                    width: 1.7rem;
                    height: .78rem;
                    line-height: .78rem;
                    font-size: .24rem;
                    color: #848486;
                    text-align: center;
                    background-color: #f4f4f4;
                    border-radius: 4px;
                    margin-right: .98rem;
                    margin-bottom: .3rem;
                }
                .blueSpan{
                    color: #6da5e2;
                    background-color: #eaf3fc;
                }
                .greenSpan{
                    color: #6aba9e;
                    background-color: #e5f6f0;
                }
                .orangeSpan{
                    color: #e8b96b;
                    background-color: #fcf5eb;
                }
                span:nth-child(3){
                    margin-right: 0;
                }
            }
        }
    }
    .label_btn{
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        button{
            flex: 1;
            border: none;
            background: none;
            outline: none;
            height: .8rem;
        }
        .reset{
            background-color: #f6f6f6;
            color: #5193f3;
        }
        .confirm{
            background-color: #5193f3;
            color:#FFF;
        }
    }
	.label_list li .list span.active{
		background-color: #FFF7EB;
		color: #F1A972;
	}
	.label_list li.blueTitle .list span.active{
		background-color: #EBF3FF;
		color: #7AACE6;
	}
	.label_list li.greenTitle .list span.active{
		background-color: #E4F7F3;
		color: #65BC9B;
	}
</style>
