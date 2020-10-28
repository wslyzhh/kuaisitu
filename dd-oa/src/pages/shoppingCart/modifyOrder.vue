<!-- 修改订单 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">{{formData.orderID}}</h3>
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
                <label class="title"><span>合同造价</span></label>
                <input type="text" :value="formData.o_contractprice" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动名称</span></label>
                <input type="text" v-model="formData.o_content" placeholder="请输入活动名称">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动地点</span></label>
                <input type="text" v-model="formData.o_address" placeholder="请输入活动地点">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectRangeDate">
                <label class="title"><span>活动日期</span></label>
                <input type="text" :value="date_range" readonly placeholder="请选择活动日期">
                <div class="icon_right time"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeArea">
                <label class="title"><span>归属地</span></label>
                <input type="text" :value="placeText" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="staff(1,'employee1')">
                <label class="title"><span>报账人员</span></label>
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
                <input type="text" readonly :value="formData.fstatus_text">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changePushstatus">
                <label class="title"><span>推送上级审核</span></label>
            	<input type="text" readonly :value="formData.o_isPush_text">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="formData.o_remarks" placeholder="请输入备注"></textarea>
            </li>
        </ul>
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>一类活动文件</span></label>
				<div class="icon_right accessory" v-if="formData.o_lockStatus == 'False'">
					<input type="file" multiple="multiple" @change="upFile($event,'1')">
				</div>
            </li>
			<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files1" :key="index">
				<a :href="'/' + f.f_filePath">{{f.f_fileName}}</a>
				<span>{{f.f_size}}K</span>
			    <div class="icon_right delete" v-if="formData.o_lockStatus == 'False'" 
				@click="delOrderFile('1',f.f_id,f.f_fileName)"></div>
			</li>
        </ul>
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>二类活动文件</span></label>
                <div class="icon_right accessory" v-if="formData.o_lockStatus == 'False'">
					<input type="file" multiple="multiple" @change="upFile($event,'2')">
				</div>
            </li>
			<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files2" :key="index">
				<a :href="'/' + f.f_filePath">{{f.f_fileName}}</a>
				<span>{{f.f_size}}K</span>
			    <div class="icon_right delete" v-if="formData.o_lockStatus == 'False'" 
			    @click="delOrderFile('2',f.f_id,f.f_fileName)"></div>
			</li>
        </ul>
        <div class="hint">
            部分人员可查看，文件大小限制：51200KB</br> 
            文件类型：gif,jpg,jpeg,png,bmp,rar,zip,doc,xls,txt,docx,xlsx
        </div>
		<ul class="button_list">
			<router-link tag="li" :to="{path:'/invoiceslist',query:{oID:formData.orderID}}" style="background-color:#47a21f;">发票申请汇总</router-link>
            <router-link tag="li" :to="{path:'/settlement',query:{oID:formData.orderID}}" style="background-color:#008265;">结算汇总</router-link>
			<router-link tag="li" :to="{path:'/unBusiness',query:{oID:formData.orderID}}" style="background-color:#008265;">执行备用金借款明细</router-link>
		</ul>
		<choose :show.sync="showChoose" :showNum="showNum"  :type="chooseType" :list="chooseList" @on-affirm="activeChoose"></choose>
		<ul class="looK_button_list c_flex">
            <router-link tag="li" :to="{path:'/UnBusinessPayAdd',query:{oID:formData.orderID,paytype:0,payfunction:0,type:'add'}}" style="background-color:#3395fa;">非业务申请</router-link>
            <router-link tag="li" :to="{path:'/adviceOfReceipt',query:{oID:formData.orderID,type:'add'}}" style="background-color:#47a21f;">收款通知</router-link>
            <router-link tag="li" :to="{path:'/adviceOfPayment',query:{oID:formData.orderID,type:'add'}}" style="background-color:#008265;">付款通知</router-link>
            <router-link tag="li" :to="{path:'/addInvoice',query:{oID:formData.orderID,type:'add',cName:clientName,cID:clientId}}" style="background-color:#d32c00;">发票申请</router-link>
        </ul>
		<top-nav title="编辑订单" text="保存" @rightClick="submit"></top-nav>
    </div>
</template>

<script>
	import {
		mapActions,
		mapState
	} from 'vuex'
	import choose from '@/components/choose.vue'
	import dayjs from 'dayjs'
	
	let orderId = '';
	
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
				fstatus:0,
				employee1:'',
				employee2:'',
				employee3:'',
				employee4:'',
				o_isPush:'',
				managerid:0   // TODO:测试当前登录人ID
			},
            clientList:[],
            clientName:'请选择',
			clientId:0,
            loginName:'登录人姓名',
            placeText:'',
            showChoose:false,
            date_range:'',
            showNum:false,
            chooseType:1,
            chooseEl:'',
            employee1Text:'',
            employee2Text:'',
            employee3Text:'',
            employee4Text:'',
			employeeChoose:{
				'employee1':[],
				'employee2':[],
				'employee3':[],
				'employee4':[],
			},
            chooseList:[],
			defaultStart:0,
			defaultEnd:0,
			files1:[],
			files2:[],
        };
    },
    components: {choose},
	computed: {	
        ...mapState({
			selectClientArray:status=>status.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })
    },
    created(){
		/*
        this.ddSet.setTitleRight({title:'修改订单',text:'保存'}).then(res => {
            if(res){

            }
        })
        */
    },
    mounted() {
		orderId = this.$route.query.id;
		console.log(orderId)
		this.formData.orderID = orderId;
		this.getOneOrderData()
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
    		'submitOrder',
			'getOrderDetails',
			'delFile',
        ]),
		getOneOrderData(){
			let _this = this
			this.getOrderDetails({orderID:orderId}).then(res => {
				if(!res.data){
					return;
				}
				let tmpData = res.data;
				for (var key in tmpData) { 
					if('string' == typeof tmpData[key]){
						_this.$set(_this.formData,key,tmpData[key])
					}
				}
				_this.formData.co_id = tmpData.o_coid;
				
				_this.date_range = tmpData.o_sdate + '至' + tmpData.o_edate
				_this.defaultStart = dayjs(tmpData.o_sdate).valueOf()
				_this.defaultEnd = dayjs(tmpData.o_edate).valueOf()
				
				_this.clientName = tmpData.c_name
				_this.clientId = tmpData.c_id
				
				_this.loginName = _this.getOwnerByIndex(tmpData.owner,2);
				
				// 处理归属地显示
				let source = [];
				_this.chooseEl = 'place';
				for (var key in tmpData.arealist) { 
					source.push({
						key:key,
						name:tmpData.arealist[key]
					})
				}
				_this.activeChoose(source)
				// 处理人员 1
				source = [];
				_this.chooseEl = 'employee1';
				tmpData.Employee1.map(function(item,index){
					source.push({
						de_subname:item.op_number,
						de_area:item.op_area,
						de_name:item.op_name,
					})
    			})
				_this.activeChoose(source)
				// 处理人员 3
				source = [];
				_this.chooseEl = 'employee3';
				tmpData.Employee3.map(function(item,index){
					source.push({
						de_subname:item.op_number,
						de_area:item.op_area,
						de_name:item.op_name,
					})
				})
				_this.activeChoose(source)
				// 处理人员 2
				source = [];
				_this.chooseEl = 'employee2';
				tmpData.Employee2.map(function(item,index){
					source.push({
						de_subname:item.op_number,
						de_area:item.op_area,
						de_name:item.op_name,
						dstatus:item.op_dstatus,
					})
				})
				_this.activeChoose(source)
				// 处理人员 4
				source = [];
				_this.chooseEl = 'employee4';
				tmpData.Employee4.map(function(item,index){
					source.push({
						de_subname:item.op_number,
						de_area:item.op_area,
						de_name:item.op_name,
						dstatus:item.op_dstatus,
					})
				})
				_this.activeChoose(source)
				
				_this.$set(_this.formData,'fstatus',tmpData['o_status'])
				_this.$set(_this.formData,'fstatus_text',tmpData['o_statusText'])
				
				_this.$set(_this.formData,'o_isPush',tmpData['o_ispush'])
				_this.$set(_this.formData,'o_isPush_text',_this.getIsPushText(tmpData['o_ispush']))
				
				_this.files1 = tmpData['files1']
				_this.files2 = tmpData['files2']
				
				console.log(_this.formData)
			})
		},
        submit(){   //提交
    		let _this = this
    		// 判断必填
    		if('True' == _this.formData.o_lockStatus){
				this.ddSet.setToast({text:'已锁单，不能再编辑订单信息'})
				return
			}
    		
    		this.formData.c_id = this.clientId;
			console.log(this.formData)
    		_this.submitOrder(this.formData).then(function(res){
    			if(res.data.status){
					_this.ddSet.setToast({text:'编辑订单成功'})
    			}
    			else{
					_this.ddSet.setToast({text: res.data.msg})
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
    		// if(items.length < 1){
			// 	this.ddSet.setToast({text:'请正确选择'})
    		// 	return;
    		// }
            if('place' == _this.chooseEl){
    			let tmpTexts = [];
    			let tmpPlaces = [];
    			items.map(function(item,index){
    				tmpTexts.push(item.name)
    				tmpPlaces.push(item.key)
    			})
    			_this.placeText = tmpTexts.join(',');
    			_this.$set(_this.formData,'o_place',tmpPlaces.join(','))
    			// 区域改变，清空人员
    			_this.claerEmployee();
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
    		if('employee1' == _this.chooseEl || 
    		'employee2' == _this.chooseEl || 'employee3' == _this.chooseEl || 'employee4' == _this.chooseEl){
    			let tmpStatusText = '';
    			
    			let tmpTexts = [];
    			let tmpEmployees = [];
    			let tmpGonghaos = [];
    			items.map(function(item,index){
    				tmpTexts.push(item.de_name)
					// 特殊处理 employee2 和 employee4 需要多加一个状态
					if ('employee2' == _this.chooseEl || 'employee4' == _this.chooseEl) {
						if(item['dstatus']){
							tmpStatusText = '|' + item['dstatus'];
						}
						else{
							tmpStatusText = '|0';
						}
					}
    				tmpEmployees.push(item['de_name'] + '|'+item['de_subname']+'|'+item['de_area']+tmpStatusText)
    				tmpGonghaos.push(item['de_subname'])
    			})
    			_this.$set(_this.employeeChoose,_this.chooseEl,tmpGonghaos)
    			
    			_this.$set(_this,_this.chooseEl + 'Text',tmpTexts.join(','))
    			_this.$set(_this.formData,_this.chooseEl,tmpEmployees.join(','))
    		}
        },
        staff(_type,_el){ // 报账人员
            let _this = this
    		let tmpPlaces = []
    		if(_this.formData.o_place){
    			tmpPlaces = _this.formData.o_place.split(',')
    		}
    		
    		if(tmpPlaces.length < 1){
				this.ddSet.setToast({text:'请先选择活动归属地'})
    			return
    		}
    		let _isShowNum = false
            if(_el == 'employee2' || _el == 'employee4'){
                _isShowNum = true
            }
            _this.getEmployeebyarea({arealist:_this.formData.o_place,isShowNum:_isShowNum,hasOrder:orderId}).then(res => {
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
        changeArea(){    //归属地
            let _this = this
            _this.getArea().then(res => {
    			_this.chooseType = 2;
    			_this.chooseEl = 'place';
                let source = []
    			let tmpPlaces = []
    			if(_this.formData.o_place){
    				tmpPlaces = _this.formData.o_place.split(',')
    			}
                res.data.map((item,index) => {
                    source.push({
    					isChecked:tmpPlaces.includes(item.key),
    					key:item.key,
    					value:item.value,
    					name:item.value,
    				})
                })
    			_this.chooseList = source;
    			_this.showChoose = true
            })
        },
        upFile(e,_type){
			let _this = this;
			if(e.target.files.length < 1){
				this.ddSet.setToast({text:'请选择文件'})
				return
			}
            let file = e.target.files
			var data = new FormData();
			data.append("file",file)
			Object.values(e.target.files).map((item,index) => { 
				data.append("file",item)                
			}) 
			data.append("type",1)
			data.append("keyID",orderId)
			data.append("fileType",_type)
			data.append("managerid",this.formData.managerid)
            this.getUpLoadFile(data).then(res => {
        		if(1 == res.data.status){
					_this['files' + _type].push({
						f_id:res.data.fileID,
						f_fileName:res.data.name,
						f_filePath:res.data.path,
						f_size:res.data.size,
					})
				}
				else{
					_this.ddSet.setToast({text: res.data.msg})
				}
            })
        },
		delOrderFile(_type,_fid,_fileName){
			let _this = this;
			this.ddSet.setConfirm('确定要删除《'+_fileName+'》文件吗？').then(res=>{
				if(0 == result.buttonIndex){
					this.ddSet.showLoad()
					_this.delFile({
					fileID:_fid,
					type:1,
					managerid:_this.formData.managerid
					}).then(res => {
						_this.ddSet.hideLoad()
						if(1 == res.data.status){
							_this.ddSet.setToast({text:'删除文件成功'})
							_this['files' + _type] = _this['files' + _type].filter(function(item){
								return item.f_id != _fid
							})
						}
						else{
							_this.ddSet.setToast({text: res.data.msg})
						}
					}).catch(err => {
						_this.ddSet.hideLoad()
					})
				}

			})
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
                let source = []
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source}).then(res => {
                    _this.$set(_this.formData,'o_isPush',res.value)
                    _this.$set(_this.formData,'o_isPush_text',res.key)
                })
            })
        },
        changeFstatus(){
            let _this = this
            this.getFstatus({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                let source = []
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source}).then(res => {
                    _this.$set(_this.formData,'fstatus',res.value)
                    _this.$set(_this.formData,'fstatus_text',res.key)
                })
            })
        },
        changeCost(){   //合同造价
            let _this = this
            this.getContractprices({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                let source = res.data
                _this.ddSet.setChosen({source}).then(res => {
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
				this.ddSet.setToast({text:'请先选择客户'})
    			return
    		}
    		_this.getContactsbycid({c_id:_this.clientId}).then(res => {
				console.log(res)
    			_this.chooseType = 1;
    			_this.chooseEl = 'link_man';
    			let source = []
    		    res.data.map((item,index) => {
    				if(!item.name){
    				    _this.$set(item,'name',item.co_name)
    				}
    				if(this.formData.co_id == item.co_id ){
    					_this.$set(item,'isChecked',true)
    				}
    				source.push(item)
    		    })
    			_this.chooseList = source;
    		    _this.showChoose = true
    		})
    	},
		getOwnerByIndex(_owner,_index){
			let tmp = _owner.split(',');
			if(tmp.length < _index){
				return '错误下标'
			}
			
			return tmp[_index]
		},
		getIsPushText(_push){
			if('True' == _push){
				return '已推送';
			}
			else{
				return '未推送';
			}
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
    .looK_button_list{
        li{
            outline: none;
            background: none;
            border: none;
            width: 6.9rem;
            height: .8rem;
            line-height: .8rem;
            text-align: center;
            color: #FFF;
            font-size: .36rem;
            margin: 0 auto;
            border-radius: 4px;
            margin-bottom: .2rem;
        }
    }
	.button_list{
		margin: 0 0.2rem;
		li{
			outline: none;
            background: none;
            border: none;
            width: 3.5rem;
            height: .8rem;
            line-height: .8rem;
            text-align: center;
            color: #FFF;
            font-size: .36rem;
            margin: 0 auto;
            border-radius: 4px;
            margin-bottom: .2rem;
			display:inline-block;


		}
		li:nth-child(3){
			width: 6.9rem;
		}
	}
</style>

