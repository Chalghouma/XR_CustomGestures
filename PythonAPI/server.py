from flask import *
import tensorflow as tf
from neural import NeuralNet
from dataset import prepare_data
import numpy as np 

app = Flask(__name__)
row_size = 56

@app.route('/hello',methods=['GET'])
def meth():
    return "hey"

@app.route('/type',methods=['POST'])
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
        
        
        predictions = neural_network.predict_element(np_array)

        argmax = np.argmax(predictions[0])
        maxVal = np.max(predictions[0])
        return json.dumps('{"Confidence":'+str(maxVal)+',"Predicted":'+str(argmax)+'}')

@app.route('/train',methods=['GET'])
def train():
    neural_network=NeuralNet(row_size,row_size)
    neural_network.build_layers()
    
    labels_dictionary = {
    'Circle':0,'L':1,'RightArrow':2
    }      
    (training_data , training_labels ) = prepare_data('Datasets/DatasetSample0/Images',labels_dictionary,row_size,row_size)  
    neural_network.fit_data(training_data,training_labels,10,0.998)
    neural_network.save_model('StoredModel')
        
    return "Finished Fitting/Saving Model"


app.run(port=6969)
print('App is running')