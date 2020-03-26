<!-- 客户管理首页 -->
<template>
    <div>
        <div class="search_box flex flex_a_c flex_j_c">
           <input type="text" v-model="searchText" @input="changeSearch" placeholder="搜索联系人姓名或公司名称">
        </div>
        <div class="customer_list">
            <h2 class="amount">共{{list.length}}人</h2>
            <ul class="list">
                <li v-for="(item,index) in list" :key="index" @click="activeItem(item)" :class="[isSelected(item.c_id) ? 'active':'']">
                    <div class="company flex flex_a_c flex_s_b">
                        <section class="flex flex_a_c">
                            <img class="icon" :src="isIocn[item.c_flag]" alt="">
                            <h2 class="name">{{item.c_name}}</h2>
                            <!-- <input type="button" :class="{blue:item.c_isUse}" :value="item.c_isUse?'启用': '禁用'"> -->
                        </section>
                    </div>
                    <!-- <div class="message flex flex_a_c flex_s_b">
                        <div class="message_list flex">
                            <span v-show="item.c_num">{{item.c_num}}</span>
                            <span>{{item.co_name}}</span>
                            <span>{{item.co_number}}</span>
                        </div>
                    </div> -->
                </li>
            </ul>
        </div>
		<div class="choose_btn_box">
			<div class="choose_btn">
			    <!-- <button @click="chageReset">重置</button> -->
			    <button @click="selectCancel">取消</button>
			    <button class="affirm" @click="selectClient">确认</button>
			</div>
		</div>
		<top-nav title="客户选择"></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import {mapActions,mapState} from 'vuex'

import audit from '../../assets/img/audit.png'
import audit_no from '../../assets/img/audit_no.png'
import audit_yes from '../../assets/img/audit_yes.png'
import * as dd from 'dingtalk-jsapi'

export default {
    name:"",
    data() {
       return {
           topTabList:['普通客户','管理用客户','内部客户'],
           tabIndex:0,
           list:[],
           selectList:[],
           isIocn:[audit,audit_no,audit_yes],
           searchText:'',
		   selectType:1  ,// 选择模式，1 单选 ；2 多选
		   selectMax:0    // 多选模式下，最多可选多少个，0 不限
       };
    },
    components: {
        tabList,
    },
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })    
	},
    created(){

    },
    mounted() {
        this.customerList()
		this.selectList = [this.$route.query.selected_id]
    },
    methods: {
        ...mapActions([
            'getCustomerList'
        ]),
		isSelected:function(_id){
			return this.selectList.includes(_id);
		},
		activeItem(item){
			if(1 == this.selectType){
				this.selectList = [item.c_id];
			}
			else{
				if(this.selectList.includes(item.c_id)){
					this.selectList = this.selectList.filter(function(el){
						return el != item.c_id;
					})
				}
				else{
					if(this.selectMax > 0 && this.selectList.length > (this.selectMax - 1)){
						dd.device.notification.alert({
							message: '最多允许选择'+this.selectMax+'个',
							title: "",//可传空
							buttonName: "知道了",
							onSuccess : function() {
								//onSuccess将在点击button之后回调
								/*回调*/
							},
							onFail : function(err) {}
						});
						return;
					}
					this.selectList.push(item.c_id);
				}
			}
		},
        chageReset(){
            this.selectList = [];
        },
		selectCancel(){
			this.$router.go(-1)
		},
        selectClient(){
            let activeList = []
			let _this = this;
            _this.list.map((item,index) => {
                if(_this.isSelected(item.c_id)){
                    activeList.push({
						name:item.c_name,
						id:item.c_id,
						co_number:item.co_number,
						co_name:item.co_name,
						co_id:item.co_id
					})
                }
            })
			// console.log(activeList)
			this.$store.dispatch("changeSelectClient", activeList)
			setTimeout(function() {
				_this.$router.go(-1)
			}, 100);
        },
        customerList({type = 1} = {}){
            let params = {
                pageIndex:1,
                pageSize:999,
                keywords:this.searchText,
                type,
                managerid:this.userInfo.id
            }
            this.getCustomerList(params).then(res => {
                this.list = res.data.list
            })
        },
        changeSearch(){
            this.customerList({type:this.tabIndex+1})
        }
    },
}
</script>

<style scoped lang="scss">
@import '../../assets/css/var.scss';
    .tab_list{
        border-bottom: 1px solid $border_bottom_color;
        li{
            height: .75rem;
            line-height: .75rem;
            color:$fontSizeColor;
            font-size: $size_30;
        }
        .active{
            color:$blue_1;
            font-weight: bold;
            border-bottom: 2px solid $blue_1;
        }
    }
    .search_box{
        height: .88rem;
        input{
            width: 7rem;
            height: .6rem;
            text-indent: .5rem;
            background-color: #ededed;
            border-radius: 4px;
            font-size: $size_28;
            background-image: url('../../assets/img/search.png');
            background-repeat: no-repeat;
            background-size: .3rem;
            background-position:.1rem;
        }
    }
    .customer_list{
		margin-bottom:50px;
        .amount{
            height: .65rem;
            line-height: .65rem;
            padding: 0 .3rem;
            font-size: $size_28;
            color: $gery_1;
            font-weight: normal;
            background-color: $gery_3;
        }
        .list{
            .company{
                padding: 0 .3rem;
                height: .65rem;
                .icon{
                    width:$size_30;
                    height: $size_30;
                    background-color: $gery_3;
                    margin-right: $size_20;
                }
                .name{
                    color: $blue_1;
                    font-size: $size_24;
                }
                input{
                    width: .86rem;
                    height: .36rem;
                    line-height: .36rem;
                    margin-left: .1rem;
                    font-size: $size_20;
                    color: #FFF;
                    border-radius: .04rem;
                    background-color: $gery_2;
                }
                .blue{
                    background-color: $blue_1;
                }
                .operation_icon{
                    span{
                        width: $size_30;
                        height: $size_30;
                        margin-left: .3rem;
                        background-repeat: no-repeat;
                        background-position: center;
                        background-size: cover;
                    }
                    span:nth-child(1){
                        background-image: url('../../assets/img/redact_1.png');
                    }
                    span:nth-child(2){
                        background-image: url('../../assets/img/delete.png');
                        background-size: .25rem .3rem;
                    }
                }
            }
            .message{
                padding: 0 .3rem;
                height: .65rem;
                background-color: $gery_3;
                .icon{
                    width: $size_30;
                    height: $size_30;
                    background-color: $blue_1;
                }
                .message_list{
                    display: flex;
                    span{
                        height: .36rem;
                        font-size: $size_20;
                        min-width: 1.22rem;
                        border-radius: 2px;
                        text-align: center;
                        line-height: .36rem;
                        margin-right: .2rem;
                    }
                    span:nth-child(1){
                        background-color: $blue_1;
                        color: #FFF;
                        padding: 0 .2rem;
                    }
                    span:nth-child(2){
                        background-color: #fff4e9;
                        color: #e1bc94;
                    }
                    span:nth-child(3){
                        background-color: #e3f6f0;
                        color: #72a795;
                        width: 1.8rem;
                    }
                }
            }
        }
    }
	.list li{
		position: relative;
		//padding: 15px 0;
		font-size: .28rem;
		color: #333;
		padding-right: .6rem;
		border-bottom: 1px solid #f1f1f1;
		&:after{
		    content: '';
		    position: absolute;
		    right:.2rem;
		    top: 50%;
		    transform: translateY(-50%);
		    width: .35rem;
		    height: .35rem;
		    border-radius: 50%;
		    border: 1px solid #CCC;
		    box-sizing: border-box;
		}
	}
	.list li.active::after{
				content: '';
				background-image: url('../../assets/img/icon_choose.png');
				background-size: .34rem;
				background-repeat: no-repeat;
				background-position: center;  
				border: none;     
			}
	.choose_btn_box{position:fixed;bottom:0;width:100%;}
	.choose_btn{
	    display: flex;
	    justify-content: space-around;
	    border-top: 1px solid #f1f1f1;
	    button{
	        outline: none;
	        background: none;
	        border: none;
	        flex: 1;
	        height: .8rem;
	        line-height: .8rem;
	        color:#333;
	        font-size: .3rem;
	        background-color: #f7f7f7;
	        &.affirm{
	            background-color: rgb(79,148,241);
	            color: #FFF;
	        }
	    }
	}
</style>


