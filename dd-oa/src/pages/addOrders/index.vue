<!-- 新增订单 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">系统自动生成</h3>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeClient">
                <label class="title"><span class="must">客户</span></label>
                <input type="text" :value="clientName" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c" @click="selectLinkMan">
                <label class="title"><span>联系人</span></label>
                <input type="text" readonly :value="formData.co_name" placeholder="请选择客户">
				<div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>联系号码</span></label>
                <input type="text"  readonly :value="formData.co_number" placeholder="请选择客户">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeCost">
                <label class="title"><span class="must">合同造价</span></label>
                <input type="text" :value="formData.o_contractprice" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">活动名称</span></label>
                <input type="text" v-model="formData.o_content" placeholder="请输入活动名称">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">活动地点</span></label>
                <input type="text" v-model="formData.o_address" placeholder="请输入活动地点">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectRangeDate">
                <label class="title"><span class="must">活动日期</span></label>
                <input type="text" :value="date_range" readonly placeholder="请选择活动日期">
                <div class="icon_right time"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeArea">
                <label class="title"><span class="must">归属地</span></label>
                <input type="text" :value="placeText" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="staff(1,'employee0')">
                <label class="title"><span class="must">业务员</span></label>
                <input type="text" :value="employee0Text" readonly>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="staffCommon()">
                <label class="title" style="width: 1.7rem;"><span>共同业务员</span></label>
                <input type="text" :value="employee6Text" readonly>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="staff(1,'employee1')">
                <label class="title"><span class="must">报账人员</span></label>
                <input type="text" :value="employee1Text" readonly>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="staff(2,'employee2')">
                <label class="title"><span>策划人员</span></label>
				<input type="text" :value="employee2Text" readonly>
                <div class="icon_right add"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="staff(2,'employee3')">
                <label class="title"><span>执行人员</span></label>
				<input type="text" :value="employee3Text" readonly>
                <div class="icon_right add"></div>
            </li>
			<li class="flex flex_a_c flex_s_b" @click="staff(2,'employee4')">
			    <label class="title"><span>设计人员</span></label>
				<input type="text" :value="employee4Text" readonly>
			    <div class="icon_right add"></div>
			</li>
			<li class="flex flex_a_c flex_s_b" @click="changeFstatus">
			    <label class="title"><span>订单状态</span></label>
			    <input type="text" readonly :value="o_status_text">
			    <div class="icon_right arrows_right"></div>
			</li>
			<li class="flex flex_a_c flex_s_b" @click="changePushstatus">
			    <label class="title"><span>推送状态</span></label>
				<input type="text" readonly :value="o_isPush_text">
			    <div class="icon_right arrows_right"></div>
			</li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="formData.o_remarks" placeholder="请输入备注"></textarea>
            </li>
        </ul>
        <choose :show.sync="showChoose" :showNum="showNum" :type="chooseType" :list="chooseList" @on-affirm="activeChoose"></choose>
        <chooseCommon :show.sync="showChoose1" :list="chooseList" @on-affirm="activeChoose1"></chooseCommon>
        <areachoose :show.sync="showChoose2" :list="chooseList" @on-affirm="activeChoose2"></areachoose>
        <top-nav title="新增订单" text="保存" @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {
	mapActions,
	mapState
} from 'vuex'
import choose from '../../components/choose.vue'
import areachoose from '../../components/areachoose.vue'
import chooseCommon from '../../components/chooseCommon.vue'
import dayjs from 'dayjs'
import Areachoose from '../../components/areachoose.vue'

export default {
    name:"",
    data() {
        return {
            formData:{
				orderID:'',
				c_id:0,
				co_id:0,
				o_contractprice:'',
				o_sdate:'',
				o_edate:'',
				o_address:'',
				o_content:'',
				o_contractcontent:'',
				o_remarks:'',
				employee0:'',
				employee1:'',
				employee2:'',
				employee3:'',
				employee4:'',
				employee6:'',
				managerid:0   // TODO:测试当前登录人ID
			},
            fstatus:0,
            o_status_text:'待定',
            o_isPush:'False',
            o_isPush_text:'未推送',
            clientList:[],
            clientName:'请选择',
			clientId:0,
            loginName:'登录人姓名',
            placeText:'',
            showChoose:false,
            showChoose1:false,
            showChoose2:false,
            date_range:'',
            chooseType:1,
            showNum:false,
            chooseEl:'',
            employee0Text:'',
            employee1Text:'',
            employee2Text:'',
            employee3Text:'',
            employee4Text:'',
            employee6Text:'',
			employeeChoose:{
				'employee0':[],
				'employee1':[],
				'employee2':[],
				'employee3':[],
				'employee4':[],
				'employee6':[]
			},
            chooseList:[],
            arealist:[]
        };
    },
    components: {choose,chooseCommon, areachoose},
    computed: {
        ...mapState(            
            {
            selectClientArray:state => state.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })
        
    },
    created(){
       let _this = this
       //默认下单人
       _this.employee0Text= _this.userInfo.real_name
       _this.formData.employee0=_this.userInfo.real_name+'|'+_this.userInfo.user_name+'|'+_this.userInfo.area
       //默认活动归属地       
        _this.getArea().then(res => {            
            res.data.map((item,index) => {
                if(_this.userInfo.area == item.key)
                //if('HQ' == item.key)
                {
                    _this.formData.o_place = item.key+'-'+item.value+'-100-1'
                    _this.placeText=item.value+'100%'
                    _this.$set(item,'type','1')
                    _this.$set(item,'ratio',100)
                }
                else{
                    _this.$set(item,'type','0')
                }
                _this.arealist.push(item)
            })
        })
    },
    mounted() {
        this.clientCallBack(this.selectClientArray)
    },
    methods: {
        ...mapActions([
            'getAllcustomer',
            'getContractprices',
            'getContactsbycid',
            'getUpLoadFile',
            'getPushstatus',
            'getFstatus',
            'getDstatus',
            'getArea',
            'getEmployeebyarea',
			'submitOrder'
        ]),
        submit(){   //提交        
			let _this = this
			// 判断必填
			if(!_this.clientId){
                _this.ddSet.setToast({text:'请选择客户'})
                return
            }
            if(!_this.formData.o_contractprice){
                _this.ddSet.setToast({text:'请选择合同造价'})
                return
            }
            if(!_this.formData.o_content){
                _this.ddSet.setToast({text:'请输入活动名称'})
                return
            }
            if(!_this.formData.o_address){
                _this.ddSet.setToast({text:'请输入活动地点'})
                return
            }
            if(!_this.date_range){
                _this.ddSet.setToast({text:'请选择活动日期'})
                return
            }
            if(!_this.formData.o_place){
                _this.ddSet.setToast({text:'请选择活动归属地'})
                return
            }
            _this.formData.fstatus=_this.fstatus
            _this.formData.o_isPush=_this.o_isPush

            _this.formData.c_id = _this.clientId;
            _this.formData.managerid=_this.userInfo.id
            _this.ddSet.showLoad()
			_this.submitOrder(_this.formData).then(function(res){
                _this.ddSet.hideLoad()
				if(res.data.status){
                    _this.ddSet.setToast({text:'新增订单成功，正在跳转...'}).then(res => {
                        //this.$router.go(-1)
                        //成功后跳转订单列表
                        _this.$router.push('./shoppingCart')
                    })
				}
				else{                    
                    _this.ddSet.setToast({text:res.data.msg})
				}
			})
        },
		claerEmployee(){ // 清空人员
			let _this = this
			let employees = ['1','2','3','4'];
			employees.map(function(item,index){
				_this.$set(_this,'employee' + item + 'Text','')
				_this.$set(_this.formData,'employee' + item,'')
			});
		},
        activeChoose(items){
			let _this = this
			if(items.length < 1){
                _this.ddSet.setToast({text:'请正确选择'})
				return;
			}
			if('link_man' == _this.chooseEl){
				if(items.length){
					_this.$set(_this.formData,'co_name',items[0].co_name)
					_this.$set(_this.formData,'co_number',items[0].co_number)
					_this.$set(_this.formData,'co_id',items[0].co_id)
				}
				else{
					_this.$set(_this.formData,'co_name','')
					_this.$set(_this.formData,'co_number','')
					_this.$set(_this.formData,'co_id',0)
				}
			}
			if('employee0' == _this.chooseEl || 'employee1' == _this.chooseEl || 
			'employee2' == _this.chooseEl || 'employee3' == _this.chooseEl || 'employee4' == _this.chooseEl){
				let tmpStatusText = '';
				// 特殊处理 employee2 和 employee4 需要多加一个状态
				if ('employee2' == _this.chooseEl || 'employee4' == _this.chooseEl) {
					tmpStatusText = '|0';
				}
				
				let tmpTexts = [];
				let tmpEmployees = [];
				let tmpGonghaos = [];
				items.map(function(item,index){
					tmpTexts.push(item.de_name)
					tmpEmployees.push(item['de_name'] + '|'+item['de_subname']+'|'+item['de_area']+tmpStatusText)
					tmpGonghaos.push(item['de_subname'])
				})
				_this.$set(_this.employeeChoose,_this.chooseEl,tmpGonghaos)
				
				_this.$set(_this,_this.chooseEl + 'Text',tmpTexts.join(','))
                _this.$set(_this.formData,_this.chooseEl,tmpEmployees.join(','))

                //选择下单人时，变更活动归属地
                if('employee0' == _this.chooseEl){
                    _this.arealist=[]
                    items.map(function(e,i){
                        _this.getArea().then(res => {            
                            res.data.map((item,index) => {
                                if(e['de_area'] == item.key)
                                {
                                    _this.formData.o_place = item.key+'-'+item.value+'-100-1'
                                    _this.placeText=item.value+'100%'
                                    _this.$set(item,'type','1')
                                    _this.$set(item,'ratio',100)
                                }
                                else{
                                    _this.$set(item,'type','0')
                                }
                                _this.arealist.push(item)
                            })
                        })
                    })                    
                }
			}
        },
        staff(_type,_el){ // 报账人员
            let _this = this
            let _isShowNum = false
            let _arealist = ''
            if(_el == 'employee2' || _el == 'employee4'){//策划人员和设计人员
                _isShowNum = true
            }
            else if(_el != 'employee0'){//报账人员和执行人员
                let tmpPlaces = []
                if(_this.formData.o_place){
                    tmpPlaces = _this.formData.o_place.split(',')
                }
                if(tmpPlaces.length < 1){
                    _this.ddSet.setToast({text:'请先选择活动归属地'})
                    return
                }
                let placelist = _this.formData.o_place.split(',')
                let tmpplace=[]
                placelist.map((e,i)=>{
                    tmpplace.push(e.split('-')[0])
                })
                _arealist = tmpplace.join(',')
            }
            _this.getEmployeebyarea({arealist:_arealist,isShowNum:_isShowNum,hasOrder:''}).then(res => {
                _this.chooseType = _type;
                _this.showNum = _isShowNum;
				_this.chooseEl = _el;
				let gonghaos = _this.employeeChoose[_el];
				let source = []
                res.data.map((item,index) => {
					if(4 == item.de_type){
						if(!item.name){
                            _this.$set(item,'name',item.de_name)
                            if(_isShowNum){
                                _this.$set(item,'orderCount',item.orderCount)
                            }
						}
						if(gonghaos.includes(item.de_subname)){
							_this.$set(item,'isChecked',true)
						}
                        source.push(item)
					}
                })
				_this.chooseList = source;
                _this.showChoose = true
            })
        },
        activeChoose1(items){
			let _this = this
			let tmpTexts = [];
            let tmpEmployees = [];
            items.map(function(item,index){
                tmpTexts.push(item.de_name+item.ratio+'%')
                tmpEmployees.push(item['de_name'] + '-'+item['de_subname']+'-'+item['de_area']+'-'+item['ratio'])
            })            
            _this.$set(_this,'employee6Text',tmpTexts.join(','))
            _this.$set(_this.formData,'employee6',tmpEmployees.join(','))
        },
        staffCommon(){
            let _this = this
            _this.getEmployeebyarea({}).then(res => {
				let source = []
                res.data.map((item,index) => {
					if(4 == item.de_type){		
                        let _chooselist = _this.formData.employee6.split(',')
                        _chooselist.map((l,i)=>{
                            let u = l.split('-')
                            if(u[1]==item.de_subname){
                                item['ratio'] = u[3]
                            }
                        })				
                        source.push(item)
					}
                })
				_this.chooseList = source;
                _this.showChoose1 = true
            })
        },
        changeArea(){    //归属地
            let _this = this
            let source = []
            _this.arealist.map((item,index) => {
                    source.push({
						key:item.key,
						value:item.value,
						name:item.value,
                        ratio:item.ratio,
                        type:item.type
					})
                })
            _this.chooseList = source
            _this.showChoose2 = true
        },
        activeChoose2(items){
            let _this = this
            _this.arealist = items  
            let tmpTexts = [];
            let tmpEmployees = [];
            _this.arealist.map(function(item,index){
                if(item.ratio && item.ratio >=0){
                    tmpTexts.push(item.value+item.ratio+'%')
                    tmpEmployees.push(item.key + '-'+item.value+'-'+item.ratio+'-'+item.type)
                }                
            })          
            _this.$set(_this,'placeText',tmpTexts.join(','))
            _this.$set(_this.formData,'o_place',tmpEmployees.join(','))

        },
        changeClient(){ //选择客户
			let _this = this;
			// _this.clientName = '123123'
			// return;
			_this.$router.push({ path: '/addOrders/customerSelect', query: { selected_id: _this.clientId }})
        },
		clientCallBack:function(_selectData){
			if(_selectData.length){
				this.clientName = _selectData[0].name;
				this.clientId = _selectData[0].id;
				this.$set(this.formData,'co_name',_selectData[0].co_name)
				this.$set(this.formData,'co_number',_selectData[0].co_number)
				this.$set(this.formData,'co_id',_selectData[0].co_id)
			}
			else{
				this.clientName = '请选择';
				this.clientId = 0;
				this.$set(this.formData,'co_name','')
				this.$set(this.formData,'co_number','')
				this.$set(this.formData,'co_id',0)
			}
		},
		changePushstatus(){
            let _this = this
            this.getPushstatus({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                let source = [],selectedKey = _this.o_isPush_text
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this,'o_isPush',res.value)
                    _this.$set(_this,'o_isPush_text',res.key)
                })
            })
        },
        changeFstatus(){
            let _this = this
            this.getFstatus({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                let source = [],selectedKey = _this.o_status_text
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this,'fstatus',res.value)
                    _this.$set(_this,'o_status_text',res.key)
                })
            })
        },        
        changeCost(){   //合同造价
            let _this = this
            this.getContractprices({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                let source = res.data
                let selectedKey = _this.formData.o_contractprice
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.formData,'o_contractprice',res.key)
                })
            })
        },
      selectRangeDate(){ //活动日期
            let _this = this
            _this.ddSet.setChooseInterval({}).then(res => {
                var sdate = dayjs(res.start).format('YYYY-MM-DD');
                var edate = dayjs(res.end).format('YYYY-MM-DD');
                _this.date_range = sdate + '至' + edate;
                _this.$set(_this.formData,'o_sdate',sdate)
                _this.$set(_this.formData,'o_edate',edate)
            })  
        },
		selectLinkMan(){
			let _this = this
			if(_this.clientId < 1){
                _this.ddSet.setToast({text:'请先选择客户'})
				return
			}
			_this.getContactsbycid({c_id:_this.clientId}).then(res => {
				_this.chooseType = 1;
				_this.chooseEl = 'link_man';
				let source = []
			    res.data.map((item,index) => {
					if(!item.name){
					    _this.$set(item,'name',item.co_name)
					}
					if(_this.formData.co_id == item.co_id ){
						_this.$set(item,'isChecked',true)
					}
					source.push(item)
			    })
				_this.chooseList = source;
			    _this.showChoose = true
			})
		}
    },
}
</script>

<style scoped lang='scss'>
    
    .hint{
        padding: .3rem;
        font-size: .24rem;
        color: #fc0214;
        line-height: .4rem;
    }
    
</style>
