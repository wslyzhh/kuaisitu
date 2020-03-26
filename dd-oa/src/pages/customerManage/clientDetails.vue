<!-- 客户详情 -->
<template>
    <div class="body_gery" v-if="isDatails">
        <div class="details">
            <div class="company_message flex">
                <div class="company">
                    <div class="message flex">
                        <div class="icon"></div>
                        <h3 class="name">{{datails.c_name}}</h3>
                        <img class="compile" src="../../assets/img/redact_2.png" alt="" 
                        @click="goPage('addClient')">
                    </div>
                    <div class="code flex">
                        <span>{{clientType[datails.c_type-1]}}</span>
                        <span v-show="datails.c_num">{{datails.c_num}}</span>
                        <span>{{datails.c_isUse ? '启用' : '禁用'}}</span>
                        <img class="yes" :src="isIocn[datails.c_flag]" alt="">
                    </div>
                    <p class="introduce">{{datails.c_remarks}}</p>
                </div>
            </div>
            <div class="client_massage">
                <h2 class="name">{{datails.contacts_list[0].co_name}}</h2>
                <p class="tel">{{datails.contacts_list[0].co_number}}</p>
                <img class="compile" src="../../assets/img/redact_2.png" alt=""  @click="goPage('addLinkman')">
            </div>
        </div>
        <tab-list :tabList="topTabList" @on-tab="changeTab"></tab-list>
        <div class="client_list" v-if=" this.tabIndex==0">
            <h2 class="amount">共{{datails.contacts_list.length-1}}人<span class="addbtn" @click.stop="addLinkman">添加联系人</span></h2>
            <ul class="list">
                <li class="flex flex_a_c" v-if="!item.co_flag" v-for="(item,index) in datails.contacts_list" :key="index">
                    <div class="head">
                        <img src="../../assets/img/head.png" alt="">
                    </div>
                    <div class="message">
                        <h2 class="name">{{item.co_name}}</h2>
                        <p class="tel">
                            {{item.co_number}}
                        </p>
                    </div>
                    <img class="compile" src="../../assets/img/redact_1.png" @click="goPage('addLinkman',item)" alt="">
                    <img class="compile1" src="../../assets/img/delete.png" @click="delLinkman(item.co_id)" alt="">
                </li>
            </ul>
        </div>
        <div class="bank_list" v-if="this.tabIndex==1">
            <h2 class="amount">共{{datails.banks_list.length}}条<span class="addbtn" @click.stop="addBank">添加银行账号</span></h2>
            <ul class="list">
                <li class="flex flex_a_c" v-for="(item,index) in datails.banks_list" :key="index">
                    <div class="head">
                        <img src="../../assets/img/head.png" alt="">
                    </div>
                    <div class="message">
                        <h2 class="name">{{item.cb_bankName}}</h2>
                        <p class="tel">
                            {{item.cb_bankNum}}
                        </p>
                    </div>
                    <img class="compile" src="../../assets/img/redact_1.png" @click="goPage('addBank',item,index)" alt="">
                    <img class="compile1" src="../../assets/img/delete.png" @click="delBank(item.cb_id)" alt="">
                </li>
            </ul>
        </div>
        <top-nav title="客户详情" text="添加联系人" @rightClick="addLinkman"></top-nav>
    </div>
</template>

<script>
import tabList from '../../components/tab.vue'
import { mapActions,mapState } from 'vuex';

import audit from '../../assets/img/audit.png'
import audit_no from '../../assets/img/audit_no.png'
import audit_yes from '../../assets/img/audit_yes.png'
export default {
    name:"",
    data() {
       return {
           tabIndex:0,
           topTabList:['联系人','银行账号'],
           clientId:null,
           isIocn:[audit,audit_no,audit_yes],
           datails:{},
           isDatails:false,
           clientType:['普通客户','管理用客户','内部客户']
       };
    },
    components: {
        tabList
    },
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        
    },
    created(){
        let {id} = this.$route.query
        this.clientId = id
        this.ddSet.showLoad()
        this.getCustomerDetails({c_id:id}).then(res => {
            this.ddSet.hideLoad()
            this.datails = res.data
            this.isDatails = true
        }).catch(err => {
            this.ddSet.hideLoad()
        })
    },
    mounted() {
        
    },
    methods: {
        ...mapActions([
            'getCustomerDetails',
            'getContactDel',
            'getBankDel'
        ]),
        goPage(item,msg,index){
            if(item == 'addClient'){        //编辑客户
                this.$router.push({path:`/${item}`,query:{c_id:this.datails.c_id,type:'EDIT'}})
            }
            if(item == 'addLinkman' && !msg){
                this.$router.push({path:`/${item}`,query:{datails:this.datails,type:'EDIT'}})
            }
            if(item == 'addBank'){
                this.$router.push({path:`/${item}`,query:{datails:this.datails,type:'EDIT',index:index}})
            }
            if(item == 'addLinkman' && msg){
                this.$router.push({path:`/${item}`,query:{datails:this.datails,type:'msg',msg}})
            }
        },
        addLinkman(){
            this.$router.push({path:'/addLinkman',query:{datails:this.datails,type:'add'}})
        },
        delLinkman(_id){
            let _this = this;
			_this.ddSet.setConfirm('确定要删除该联系人吗？').then(res=>{
				if(0 == res.buttonIndex){
					_this.ddSet.showLoad()
					_this.getContactDel({
						co_id:_id,
						managerid:_this.userInfo.id
					}).then((res) => {
						_this.ddSet.hideLoad()
						if (res.data.status) {
							_this.ddSet.setToast({text:'成功删除联系人'})
                            _this.$router.push({path:`/clientDetails`,query:{id:_this.clientId}})
                            _this.datails.contacts_list = _this.datails.contacts_list.filter(function(item){
								return item.co_id != _id
							})
						}
						else{
							_this.ddSet.setToast({text:res.data.msg})
						}
					})
				}
            })
        },
        addBank(){
            this.$router.push({path:'/addBank',query:{datails:this.datails,type:'add'}})
        },
        delBank(_id){
            let _this = this;
			_this.ddSet.setConfirm('确定要删除该银行账号吗？').then(res=>{
				if(0 == res.buttonIndex){
					_this.ddSet.showLoad()
					_this.getBankDel({
						cb_id:_id,
						managerid:_this.userInfo.id
					}).then((res) => {
						_this.ddSet.hideLoad()
						if (res.data.status) {
							_this.ddSet.setToast({text:'成功删除银行账号'})
                            _this.datails.banks_list = _this.datails.banks_list.filter(function(item){
								return item.cb_id != _id
							})
						}
						else{
							_this.ddSet.setToast({text:res.data.msg})
						}
					})
				}
            })
        },
        changeTab(index){
            this.tabIndex = index
        }
    },
    
}
</script>

<style scoped lang='scss'>
@import '../../assets/css/var.scss';
    .details{
        background-color: #2f5a91;
        padding: .2rem;
        .company_message{
            .icon{
                width: .8rem;
                min-width: .8rem;
                height: .8rem;
                margin-right: .2rem;
                background-image: url('../../assets/img/enterprise.png');
                background-repeat: no-repeat;
                background-position: center;
                background-size: .7rem .8rem;
            }
            .company{
                position: relative;
                width: 100%;
                .compile{
                    position: absolute;
                    right: .2rem;
                    top:0;
                    width: .6rem;
                    height: .6rem;
                }
                .name{
                    font-size: $size_48;
                    color: #FFF;
                    margin-bottom: .2rem;
                    width: 5rem;
                }
                .code{
                    color: #f2cb51;
                    font-size: $size_20;
                    margin-top: .2rem;
                    span{
                        padding:.1rem;
                        border-radius: .04rem;
                        color: #FFF;
                        margin-right: .15rem;
                    }
                    .yes{
                        width: .4rem;
                        height: .4rem;
                    }
                }
                span:nth-child(1){
                    background-color: #f2cb51;
                }
                span:nth-child(2){
                    background-color: #e39206;
                }
                span:nth-child(3){
                    background-color: $blue_1;
                }
                .introduce{
                    font-size: $size_24;
                    color: #FFF;
                    line-height: .35rem;
                    margin-top: .2rem;
                }
            }
        }
        .client_massage{
            position: relative;
            margin-top: .6rem;
            .name{
                font-size: $size_48;
                color: #FFF;
            }
            .tel{
                font-size: $size_30;
                color:#FFF;
                padding-left: .6rem;
                line-height: .4rem;
                margin-top: .2rem;
                background-image: url('../../assets/img/tel.png');
                background-position: 0;
                background-repeat: no-repeat;
                background-size: .23rem .38rem;
            }
            .compile{
                position: absolute;
                right: .2rem;
                top:0;
                width: .6rem;
                height: .6rem;
            }
        }
    }
    .client_list {
        .amount{
            height: .65rem;
            line-height: .65rem;
            padding: 0 .3rem;
            font-size: $size_28;
            color: $gery_1;
            font-weight: normal;
            background-color: $gery_3;
        }
        .addbtn{
            float: right;
        }
        li{
            position: relative;
            padding: .3rem;
            background-color: #FFF;
            .head{
                width: .75rem;
                height: .75rem;
                border-radius: 50%;
                margin-right: .3rem;
                img{
                    width: 100%;
                    height: 100%;
                }
            }
            .message{
                .name,.tel{
                    color: $fontSizeColor;
                    font-size: $size_28;
                    font-weight: normal;
                }
                .tel{
                    margin-top: .2rem;
                }
            }
            .compile{
                position: absolute;
                right: .8rem;
                top: 50%;
                width: .4rem;
                height: .4rem;
                margin-top: -.3rem;
            }
            .compile1{
                position: absolute;
                right: .2rem;
                top: 50%;
                width: .4rem;
                height: .4rem;
                margin-top: -.3rem;
            }
        }
    }
    .bank_list {
        .amount{
            height: .65rem;
            line-height: .65rem;
            padding: 0 .3rem;
            font-size: $size_28;
            color: $gery_1;
            font-weight: normal;
            background-color: $gery_3;
        }
        .addbtn{
            float: right;
        }
        li{
            position: relative;
            padding: .3rem;
            background-color: #FFF;
            .head{
                width: .75rem;
                height: .75rem;
                border-radius: 50%;
                margin-right: .3rem;
                img{
                    width: 100%;
                    height: 100%;
                }
            }
            .message{
                .name,.tel{
                    color: $fontSizeColor;
                    font-size: $size_28;
                    font-weight: normal;
                }
                .tel{
                    margin-top: .2rem;
                }
            }
            .compile{
                position: absolute;
                right: .8rem;
                top: 50%;
                width: .4rem;
                height: .4rem;
                margin-top: -.3rem;
            }
            .compile1{
                position: absolute;
                right: .2rem;
                top: 50%;
                width: .4rem;
                height: .4rem;
                margin-top: -.3rem;
            }
        }
    }
</style>
