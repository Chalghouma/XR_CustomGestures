from flask import *
import tensorflow as tf
from neural import NeuralNet
from dataset import prepare_data,load_from_dataset
import numpy as np 
import os

app = Flask(__name__)
row_size = 56
labels_map = ['Circle','L','RightArrow']
@app.route('/hello',methods=['GET'])
def meth():
    return "hey"

@app.route('/recognize',methods=['POST'])
def the_method():
    if request.headers['Content-Type'] == 'text/plain':
        return "Text Message: " + request.data

    elif request.headers['Content-Type'] == 'application/json':
        return "JSON Message: " + json.dumps(request.json)

    elif request.headers['Content-Type'] == 'application/octet-stream':
        print(request.data)
        f = open('./binary', 'wb')
        f.write(request.data)
        f.close()
        return "Binary message written!"
    else:
        f = request.files['binarydata']
        f.seek(0)
        my_bytes=f.read()

        index=0
        matrix=[]
        row=[]
        for byte in my_bytes:
            normalized = (float(byte) -0)/(255-0)
            row.append(normalized)    
            if(index % row_size == (row_size-1)):
                matrix.append(row)
                row=[]
            index=index+1
        np_array = np.array([matrix],dtype=np.float64)
        
        print(np_array[0].shape)
        #print("np_array[0]. Type : {} | Value : {}".format(type(np_array[0][0][0]),np_array[0][0][0]))

        
        neural_network=NeuralNet(row_size,row_size)
        neural_network.load_model('StoredModel.modelconfig')
        
        
        arg_max , max_val = neural_network.predict_element(np_array)
        lb = labels_map[arg_max]
        print('ArgMax : {} | MaxVal : {} |Label : {}'.format(arg_max,max_val,lb))

        result = {
            'Confidence':max_val.item(),
            'Predicted':arg_max.item(),
            'Label':lb
        };
        return json.dumps(result)


@app.route('/train',methods=['GET'])
def train():
    neural_network=NeuralNet(row_size,row_size)
    neural_network.build_layers()
    
    (training_data,training_labels,label_mapper ) = load_from_dataset('DS',row_size,row_size)
    #(training_data , training_labels ) = prepare_data('Datasets/DatasetSample0/Images',labels_dictionary,row_size,row_size)  
    
    neural_network.fit_data(training_data,training_labels,10,0.998)
    neural_network.save_model('StoredModel')
        
    return "Finished Fitting/Saving Model"

@app.route('/store_image',methods=['POST'])
def store_image():
    dataset_name = request.args.get('dataset')
    gesture_type=request.args.get('gesturetype')
    image_name = request.args.get('imagename')

    f = request.files['binarydata']
    f.seek(0) 
    my_bytes=f.read()

    gesturetype_rootfolder = os.path.join(os.path.dirname(os.path.realpath('__file__')) , 'Datasets',dataset_name,gesture_type)
    if (os.path.exists(gesturetype_rootfolder) == False):
        os.makedirs(gesturetype_rootfolder)
    
    image_fullpath =os.path.join(gesturetype_rootfolder,image_name)
    with open(image_fullpath,'wb') as file_stream:
        file_stream.write(my_bytes)

    return 'Wrote the bytes at : {}'.format(image_fullpath)




train()
app.run(port=6969)