<!-- 查看订单 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">{{formData.orderID}}</h3>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">客户</span></label>
                <input type="text" :value="clientName" readonly>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>联系人</span></label>
                <input type="text" readonly :value="formData.co_name">
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>联系号码</span></label>
                <input type="text"  readonly :value="formData.co_number">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>合同造价</span></label>
                <input type="text" :value="formData.o_contractprice" readonly>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动名称</span></label>
                <input type="text" v-model="formData.o_content" readonly>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>活动地点</span></label>
                <input type="text" v-model="formData.o_address" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>活动日期</span></label>
                <input type="text" :value="date_range" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>归属地</span></label>
                <input type="text" :value="placeText" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>报账人员</span></label>
                <input type="text" :value="employee1Text" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>策划人员</span></label>
            	<input type="text" :value="employee2Text" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>执行人员</span></label>
            	<input type="text" :value="employee3Text" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>设计人员</span></label>
            	<input type="text" :value="employee4Text" readonly>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>订单状态</span></label>
                <input type="text" readonly :value="formData.fstatus_text">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>推送状态</span></label>
            	<input type="text" readonly :value="formData.o_isPush_text">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>上级审核</span></label>
            	<input type="text" readonly :value="formData.o_flagText">
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="formData.o_remarks" readonly></textarea>
            </li>
        </ul>
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>一类活动文件</span></label>
            </li>
    		<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files1" :key="index">
    			<a :href="'/' + f.f_filePath">{{f.f_fileName}}</a>
    			<span>{{f.f_size}}K</span>
    		</li>
        </ul>
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>二类活动文件</span></label>
            </li>
    		<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files2" :key="index">
    			<a :href="'/' + f.f_filePath">{{f.f_fileName}}</a>
    			<span>{{f.f_size}}K</span>
    		</li>
        </ul>
        <div class="hint">
            部分人员可查看，文件大小限制：51200KB</br> 
            文件类型：gif,jpg,jpeg,png,bmp,rar,zip,doc,xls,txt,docx,xlsx
        </div>    	
         <top-nav :title='"订单详情"' :text='"审核"' @rightClick="doCheck"  ></top-nav>
    </div>
</template>

<script>
	import {
		mapActions,
		mapState
	} from 'vuex'
	import choose from '@/components/choose.vue'
	import * as dd from 'dingtalk-jsapi'
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
				o_isPush:''
			},
            clientList:[],
            clientName:'请选择',
			clientId:0,
            loginName:'登录人姓名',
            placeText:'',
            showChoose:false,
            date_range:'',
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
    computed: {...mapState(['addOrders'])},
    created(){
		// this.ddSet.setTitleRight({title:'查看订单'}).then(res => {
        //     if(res){

        //     }
        // })
    },
    mounted() {
		orderId = this.$route.query.id;
		console.log(orderId)
		this.formData.orderID = orderId;
        this.getOneOrderData()
        
    },
    methods: {
        ...mapActions([
			'getOrderDetails'
        ]),
        doCheck(){
            this.$router.push({path:'/checkOrder',query: {id:this.formData.orderID}})
        },
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
    		console.log(items)
    		if(items.length < 1){
    			dd.device.notification.toast({
    				text: '请正确选择', //提示信息
    				onSuccess : function(result) {
    					/*{}*/
    				},
    				onFail : function(err) {}
    			})
    			return;
    		}
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
</style>

