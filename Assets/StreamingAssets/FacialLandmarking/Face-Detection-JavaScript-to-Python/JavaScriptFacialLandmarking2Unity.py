import eel
from pylsl import StreamInfo, StreamOutlet, StreamInlet

eel.init('web')

length = 24
info = StreamInfo('Certus', 'NDI', length)
outlet = StreamOutlet(info)

@eel.expose
def sendFacialLandmarksToLSL(landmarks):
    MarkersPositions = []
    i = 0
    while i < len(landmarks):
        x = landmarks[i]
        y = landmarks[i+1]
        MarkersPositions.append(x)
        MarkersPositions.append(y)
        MarkersPositions.append(0)
        i+=2
    outlet.push_sample(MarkersPositions)

eel.start('index.html', size=(300,200))