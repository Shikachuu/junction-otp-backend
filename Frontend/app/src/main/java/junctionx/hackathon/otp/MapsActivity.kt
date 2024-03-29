package junctionx.hackathon.otp

import android.Manifest
import android.content.DialogInterface
import android.content.pm.PackageManager
import android.location.Location
import android.location.LocationListener
import android.location.LocationManager
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.ProgressBar
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat

import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.SupportMapFragment
import com.google.android.gms.maps.model.*
import com.google.maps.android.PolyUtil
import org.json.JSONArray

import java.util.*
import kotlin.collections.HashMap
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.GoogleMap.OnMapLongClickListener
import android.widget.Switch
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import java.net.URL


class MapsActivity : AppCompatActivity(), OnMapReadyCallback, GoogleMap.OnMarkerClickListener {


    private var currentPositionMarker: Marker? = null
    private lateinit var mMap: GoogleMap
    lateinit var progressBar :ProgressBar
    var isQueryRunning: Boolean = false
    set(value: Boolean) {
        runOnUiThread {

            progressBar.visibility = if ( value ) View.VISIBLE else View.INVISIBLE;

        }
    }

    var needsDeposit: Boolean = false
    set(value) {
        refreshAtms()
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_maps)

        progressBar = findViewById<ProgressBar>(R.id.progressBar)


        val needsDepositSwitch = findViewById<Switch>(R.id.needs_deposit_switch)
        needsDepositSwitch.setOnCheckedChangeListener { buttonView, isChecked -> needsDeposit = isChecked}

        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        val mapFragment = supportFragmentManager
                .findFragmentById(R.id.map) as SupportMapFragment
        mapFragment.getMapAsync(this)
    }

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */


    class AtmInfo(val postion: LatLng) {
        public var estimatedTimeInMinutes: Double = 0.0;
        public var instructionText: String = "";
        public var polylines : Collection<String>? = null

    }


    fun FetchDataForDestination(destination: String): Vector<AtmInfo> {
        // http://localhost:5000/atm?origin=47.475828,19.099312&destination=47.510687,19.055810

        val currentLocationAsString = ConvertLatlongToString(currentLocation!!)
        // TODO EZTIRDAT
        var ip = "100.98.2.250"
        var url = "http://$ip:80/atm?origin=$destination&destination=$currentLocationAsString&needsdeposit=$needsDeposit"

        var jsonAsString = URL(url).readText()

        // TODO ez a mock adat, ha kell
//        Thread.sleep(2000)
//        jsonAsString = """[{"atm":{"atmPosition":"POINT(19.089672 47.478868)","streetName":"NAGYVÁRAD TÉR                                     ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJeAhDq@tBGGFFGRCl@]dBH^XkA","travelTime":6.133333333333334,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"{ex`HmmosBYjAI_@\\eBBm@FSGGFFSl@s@xC{@tEW`BAN_AzDm@`CsBfIwFrT{BfJwA~FkCbKaHzXqBtIc@|A}BvH_CdGkHfR_CrG}@~AS@g@I]C[AcAPmDf@mAb@qIpD_Bn@w@^q@f@_EbCkA~@YNaBpEgAhD{AvE}@hCQ\\IFi@R[Fy@DE@W@yIKe@AoDDyDIgBC_BEaBAiKOgCHm@Am@CMAE]Ke@Oa@KKEIs@qAiEyGEUw@eBKSCGMPiDnECGBF]d@aApB_@v@EPG`@UQ@HADAD?@","travelTime":19.866666666666667,"userInstructionsForRoute":null},"totalTravelTime":26},{"atm":{"atmPosition":"POINT(19.089672 47.478868)","streetName":"NAGYVÁRAD TÉR METRÓÁLLOMÁS                        ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJeAhDq@tBGGFFGRCl@]dBH^XkA","travelTime":6.133333333333334,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"{ex`HmmosBYjAI_@\\eBBm@FSGGFFSl@s@xC{@tEW`BAN_AzDm@`CsBfIwFrT{BfJwA~FkCbKaHzXqBtIc@|A}BvH_CdGkHfR_CrG}@~AS@g@I]C[AcAPmDf@mAb@qIpD_Bn@w@^q@f@_EbCkA~@YNaBpEgAhD{AvE}@hCQ\\IFi@R[Fy@DE@W@yIKe@AoDDyDIgBC_BEaBAiKOgCHm@Am@CMAE]Ke@Oa@KKEIs@qAiEyGEUw@eBKSCGMPiDnECGBF]d@aApB_@v@EPG`@UQ@HADAD?@","travelTime":19.866666666666667,"userInstructionsForRoute":null},"totalTravelTime":26},{"atm":{"atmPosition":"POINT(19.098792 47.474856)","streetName":"ÜLLÕI ÚT 131.                                     ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\x@n@`BtA","travelTime":2.4833333333333334,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"}mw`HucqsB[Uw@s@K@O@]@i@GiAw@CLBMhAv@DPFPFP@P@XOl@{E`Qe@hBWfAAXYbAaCrIiArDoA`Em@xBI^yAzHCd@{AjG{B|IyHpZkCtK{A`GaDjMsArFeBzGkB|H[tAwAxEy@nCeApCs@fBgH|Q]`AkBfF}@~AS@SE[Ec@Ck@H}Ep@mDvAuB|@_EbB{Av@yClBuA|@w@l@]RaCxGkB`GoArDKXQTk@VMBu@F_@F}EI{CEqCDqHMsDGs@?_EGiEGgCHyAEOAKu@Uo@_@k@e@{@iEyGQq@{@eBgCfDo@x@CGBFY^}@dBg@hAMr@UQ@FAD?FA@","travelTime":22.816666666666666,"userInstructionsForRoute":null},"totalTravelTime":25.3},{"atm":{"atmPosition":"POINT(19.09732 47.475238)","streetName":"KÖNYVES K. KRT. 34-36.                            ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^D?HAHh@Hn@XXr@h@","travelTime":2.9,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"{mw`H_aqsBiB{AoAeAiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJkClIs@xC{@tEW`BAN_AzDm@`CsBfIwFrT{@nDwCvLkCbK}CbMcCvJqBtIc@|A}BvH_CdGkHfR_CrG}@~AS@g@I]C[AcAPmDf@mAb@qIpD_Bn@w@^q@f@_EbCkA~@YNaBpEgAhD{AvE}@hCQ\\IFi@R[Fi@BO@E@W@yIKe@AoDDyDIgBC_BEaBAiKOgCHm@Am@CMAE]Ke@Oa@KKEIs@qAiEyGEUw@eBKSCGMPiDnECGBF]d@aApB_@v@EPG`@UQ@HADAD?@","travelTime":22.933333333333334,"userInstructionsForRoute":null},"totalTravelTime":25.833333333333332},{"atm":{"atmPosition":"POINT(19.087352 47.478773)","streetName":"HALLER U. 84-86.                                  ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJeAhDq@tBGGFFGRCl@]dBQr@d@rA~@tCf@~A]VOLGO","travelTime":8.533333333333333,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"ydx`Hm_osBFNNM\\Wg@_B_AuCe@sAn@yCBm@FSGGFFSl@e@pBMf@{@tEW`BAN_AzDm@`Cu@|CaFtRmBzH{@dDoAfFiGnVoCvKaA|DyAjGa@bBs@bCcBvFmA|CyFhOcBdE_CrG}@~AS@g@Iy@EqFx@mAb@sBx@gFzBuAj@w@^q@f@oBjAoAv@g@`@c@\\YNaBpEgAhDe@xAkBtFYn@m@XOBs@F]DW@yBEeGGoDDyDIgBC_BEaBAiKOgCHm@Am@CMAE]Ke@Oa@KKEIs@qAiEyGEUw@eBKSCGMPiDnECGBF]d@aApB_@v@EPG`@UQ@HAJA@","travelTime":22.233333333333334,"userInstructionsForRoute":null},"totalTravelTime":30.766666666666666},{"atm":{"atmPosition":"POINT(19.089588 47.4781)","streetName":"NAGYVÁRAD TÉR 1.                                  ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJeAhDq@tBGGFFGRCl@]dBn@`C`AbDnCnIz@w@`Ay@_@oAi@aBa@{AyA}ECKBMHK","travelTime":12.95,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"{`x`HolosBKRAH|@tCzAhFn@rBeBzAWTcA}CqBmGgA_ECINq@Ls@Bm@FSGGFFGRm@xBI^yAzHCd@{AjG{B|IyHpZkCtK{A`GaDjMsArFeBzGkB|H[tAwAxEy@nCeApCs@fBgH|Q]`AkBfF}@~AS@SE[Ec@Ck@H}Ep@mDvAuB|@_EbB{Av@yClBuA|@w@l@]RaCxGkB`GoArDKXQTk@VMBu@F_@F}EI{CEqCDqHMsDGs@?_EGiEGgCHyAEOAKu@Uo@_@k@e@{@iEyGQq@{@eBqE`G}@dBe@dACFm@N_Aq@DOENjA~@?NAD?@","travelTime":24.6,"userInstructionsForRoute":null},"totalTravelTime":37.55},{"atm":{"atmPosition":"POINT(19.074642 47.486211)","streetName":"TINÓDI UTCA 9-11.                                 ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJkClIs@xC{@tEW`BAN_AzDm@`CsBfIwFrT{@nDGEFDYhAoAfFiGnVQr@kBI_CMsAAiAA","travelTime":14.8,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"ysy`HuolsBi@EQAu@CD`F@lSvBVa@VBW{AO}CSkIu@eGc@yFc@cDQuBKoAJoLl@cBLcB\\q@JuBPeEr@SHs@b@iDdCiBhAq@j@iGrGcGlHs@r@cApAgEhFm@~@kBnCaCrD{I`O{DhGmBlCcAvAYh@_AzBGGf@eBo@rA_@v@EPG`@UQ@HADAD?@","travelTime":18.533333333333335,"userInstructionsForRoute":null},"totalTravelTime":33.333333333333336},{"atm":{"atmPosition":"POINT(19.092216 47.475556)","streetName":"GYÁLI ÚT 7.                                       ","expectedWaitTimeInMinutes":0},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^D?HAHh@Hn@XXdBrAnAdAtDvCdB~AHFZ^o@|@sCjDz@~BgAbAaAr@e@^CN?HLf@mArAYd@WT_@RUBe@N","travelTime":12.5,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"apw`Hs|osBd@OTC^SVUXe@lAsAMg@?IBOd@_@`As@fAcA{@_CiApAaDxDqMtOSe@SYmAi@u@s@[NUDGRc@vAq@tBGGFFSl@e@pBMf@{@tEW`BAN_AzDm@`Cu@|CaFtRmBzH{@dDoAfFiGnVoCvKaA|DyAjGa@bBs@bCcBvFmA|CyFhOcBdE_CrG}@~AS@g@Iy@EqFx@mAb@sBx@gFzBuAj@w@^q@f@oBjAoAv@g@`@c@\\YNaBpEgAhDe@xAkBtFYn@m@XOBs@F]DW@yBEeGGoDDyDIgBC_BEaBAiKOgCHm@Am@CMAE]Ke@Oa@KKEIs@qAiEyGEUw@eBKSCGMPiDnECGBF]d@aApB_@v@EPG`@UQ@HAJA@","travelTime":31.55,"userInstructionsForRoute":null},"totalTravelTime":44.05},{"atm":{"atmPosition":"POINT(19.065962 47.483543)","streetName":"BAKÁTS TÉR 14.                                    ","expectedWaitTimeInMinutes":1.414},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJkClIs@xC{@tEW`BAN_AzDm@`CsBfIwFrT{@nDwCvLkCbK}CbMcCvJqBtIc@|AY~@i@fBSQRPy@nCjEzKnB`FdHsE","travelTime":15.033333333333333,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"acy`HayjsBl@a@Qg@eBwF|A{ANu@j@iBDWBOn@NtA^~@Vd@RFURc@PYDJ]lBs@a@uAc@cAU_IaC_Ck@qCeAgA]eAOuCQiD]uL{@mD[eEYsDQWAk@FcH^iG\\}Bb@gBNwEt@q@NkE|CqBnAe@\\mFtFcBlBkElFm@n@mAxAaFhGqB|CaB`Ce@v@uIvN_ErG[\\aDtEq@pAg@rAGGf@eBg@`Ag@hAMr@UQ@FAD?FA@","travelTime":20.55,"userInstructionsForRoute":null},"totalTravelTime":36.99733333333333},{"atm":{"atmPosition":"POINT(19.068655 47.48192)","streetName":"FERENC KRT. 13.                                   ","expectedWaitTimeInMinutes":5.656},"routeFromDepartureToAtm":{"polyline":"ssw`HejqsBMj@f@\\f@^B^I@}@GiAw@CLBMhAv@FVLXB`@Gb@i@jBcF~QSdAARKb@_@nAiCdJkClIs@xC{@tEW`BAN_AzDm@`CsBfIwFrT{@nDwCvLkCbK}CbMcCvJqBtIc@|AY~@i@fBSQRPh@gBPi@`@_@j@WRFhDbAD]?ABEnHvBvA\\|A`@l@V","travelTime":15.2,"userInstructionsForRoute":null},"routeFromAtmToDestination":{"polyline":"_yx`HcjksBt@u@@D]lBQKu@_@cA[yBi@eGiBeAWcCu@kBu@aAS{AO}CSkIu@cEW{Ho@uF[c@AoAJsNt@_@DcB\\}APiAJeEr@SHs@b@iDdCiBhAq@j@kDpD}A`BiFnGmApAkGzHwB`DaCpDqCtEkFzI{DhGmBlCcAvAYh@_AzBGGf@eBo@rA_@v@EPG`@UQ@HAJA@","travelTime":14.816666666666666,"userInstructionsForRoute":null},"totalTravelTime":35.672666666666665}]"""
        //var rootObject = JSONObject(jsonAsString)

        var atms = Vector<AtmInfo>()

        val rootArray = JSONArray(jsonAsString)
        for (i in 0 until rootArray.length()) {
            val item = rootArray.getJSONObject(i)
            val totalTravelTime = item.getDouble("totalTravelTime")
            val instructions1 = item.getJSONObject("routeFromDepartureToAtm").optJSONObject("userInstructionsForRoute")
            val instructions2 = item.getJSONObject("routeFromAtmToDestination").optJSONObject("userInstructionsForRoute")


            val polyline1 = item.getJSONObject("routeFromDepartureToAtm").optString("polyline")
            val polyline2 = item.getJSONObject("routeFromAtmToDestination").optString("polyline")

            val instructions = "${instructions1}\nUse ATM\n${instructions2}"


            var posAsString = item.getJSONObject("atm").getString("atmPosition")
            posAsString = posAsString.substringAfter("(").substringBefore(")")


            val latlong =  posAsString.split(" ");
            val latitude = latlong[1].toDouble();
            val longitude = latlong[0].toDouble();
            val position = LatLng(latitude, longitude)

            var atmInfo = AtmInfo(position)
            atmInfo.estimatedTimeInMinutes = totalTravelTime
            atmInfo.instructionText = instructions
            atmInfo.polylines = listOf(polyline1!!, polyline2!!)


            atms.addElement(atmInfo)
        }
        return atms
    }

    fun ConvertLatlongToString(it: LatLng):String {
        return it.latitude.toString() + "," + it.longitude.toString()
    }

    override fun onMapReady(googleMap: GoogleMap) {
        mMap = googleMap

        // Add a marker in Sydney and move the camera
//        val sydney = LatLng(-34.0, 151.0)
//        mMap.addMarker(MarkerOptions().position(sydney).title("Marker in Sydney"))
//        mMap.moveCamera(CameraUpdateFactory.newLatLng(sydney))
        googleMap.setOnMarkerClickListener(this);

        val markerOptions = MarkerOptions().position(currentLocation).title("You are here")
            .icon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_GREEN))
        currentPositionMarker = mMap.addMarker(markerOptions)

        mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(currentLocation, 12.0f));


        mMap.setOnMapClickListener {
            destinationLocation = it
            refreshAtms()

        }

        googleMap.setOnMapLongClickListener(OnMapLongClickListener { latLng ->
            for (marker in atmMarkers.values) {
                if (Math.abs(marker.getPosition().latitude - latLng.latitude) < 0.05 && Math.abs(
                        marker.getPosition().longitude - latLng.longitude
                    ) < 0.05
                ) {
                    onMarkerLongClick(marker)
                    break
                }
            }
        })

        getLocation()

//        google.maps.event.addListener(map, 'click', function(event) {
//            placeMarker(event.latLng);
//        });
    }

    fun refreshAtms() {
        if (destinationLocation ==  null) {
            return
        }

        if(!isQueryRunning) {
            isQueryRunning = true

            var postitionAsString = ConvertLatlongToString(destinationLocation!!)

            Thread {
                try {
                    val atms = FetchDataForDestination(postitionAsString)
                    runOnUiThread {
                        onAtmsFetched(atms)
                        isQueryRunning = false
                    }
                } catch (e : Exception) {
                    runOnUiThread {
                        Toast.makeText(this, "Network error", Toast.LENGTH_LONG).show()
                    }
                }

            }.start()

        }
    }
    fun onMarkerLongClick(marker: Marker) {
        val atm = atmMarkers.filterValues { value -> value == marker }.keys.firstOrNull()
        if(atm != null) {
            showReportAtmDialog(atm)
        }

    }

    fun reportAtm(atm: AtmInfo) {

    }

    fun showReportAtmDialog(atm: AtmInfo) {
        val dialogClickListener = DialogInterface.OnClickListener { dialogInterface: DialogInterface, which: Int ->
            if(which == DialogInterface.BUTTON_POSITIVE) {
                reportAtm(atm)
            }
        }
        val builder = AlertDialog.Builder(this)
        builder.setMessage("Do you want to report this ATM as faulty?")
            .setPositiveButton("Yes", dialogClickListener)
            .setNegativeButton("No", dialogClickListener)
            .show()

    }

    var goolgeMapsPolylines: Collection<Polyline>? = null
    var atmMarkers: HashMap<AtmInfo, Marker> = HashMap()

    var currentPaths: Collection<String>? = null
    set(value) {
        goolgeMapsPolylines?.forEach {
            it.remove()
        }

        goolgeMapsPolylines = value?.map {
                str -> val coordinates : List<LatLng> = PolyUtil.decode(str)
                mMap.addPolyline(PolylineOptions().addAll(coordinates))
        }

    }

    var highlightedAtm: AtmInfo? = null
    set(newAtm) {
        currentPaths = newAtm?.polylines
    }

    private fun onAtmsFetched(atms: Vector<AtmInfo>) {
        atmMarkers.forEach {
            it.value.remove()
        }
        atmMarkers.clear()

        if(atms.count() == 0) {
            return
        }


        atms.forEach {

            val marker = mMap.addMarker(MarkerOptions().position(it.postion).title(it.estimatedTimeInMinutes.toString()))

            atmMarkers.set(it, marker)
        }

        FindAndMarkBestAtm(atms)


    }

    fun FindAndMarkBestAtm(atms: Vector<AtmInfo>) {
        val bestAtm = atms.minBy {
            it.estimatedTimeInMinutes
        }


        atmMarkers[bestAtm]?.setIcon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_BLUE))
        currentPaths = bestAtm?.polylines

    }

    var locationManager : LocationManager? = null
    var currentLocationAsString: String? = null

    fun getLocation() {

        locationManager = getSystemService(LOCATION_SERVICE) as LocationManager?

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION)
                != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(
                    this,
                    arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
                    PERMISSION_REQUEST_ACCESS_FINE_LOCATION)
            return
        }

        locationManager!!.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 0L, 0f, locationListener)
    }

    var currentLocation : LatLng = LatLng(47.475312, 19.099319)
    set(value) {
        runOnUiThread {
            currentPositionMarker?.position = value
        }
    }

    var destinationLocation : LatLng? = null

    override fun onMarkerClick(marker: Marker?): Boolean {
        //val atmMarkers = atmMarkers.mapValues { v -> v }
        val atm: AtmInfo? = atmMarkers.filterValues { _marker -> _marker == marker }.keys.singleOrNull()
        if(atm != null) {
            ShowRouteForAtm(atm)
            //return true
        }
        return false
        //atmMarkers.values.find { _marker -> _marker == marker }
    }

    fun ShowRouteForAtm(atm: AtmInfo) {
        atmMarkers[atm]?.setIcon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_YELLOW))
        currentPaths = atm.polylines
    }

    var locationListener = object : LocationListener{
        override fun onLocationChanged(location: Location?) {
            var latitute = location!!.latitude
            var longitute = location!!.longitude
            val location = LatLng(latitute, longitute)


            currentLocation = location
            mMap.moveCamera(CameraUpdateFactory.newLatLng(currentLocation))




//            Log.i("test", "Latitute: $latitute ; Longitute: $longitute")

        }

        override fun onStatusChanged(provider: String?, status: Int, extras: Bundle?) {
        }

        override fun onProviderEnabled(provider: String?) {
        }

        override fun onProviderDisabled(provider: String?) {
        }

    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == PERMISSION_REQUEST_ACCESS_FINE_LOCATION) {
            when (grantResults[0]) {
                PackageManager.PERMISSION_GRANTED -> getLocation()
                //PackageManager.PERMISSION_DENIED -> //Tell to user the need of grant permission
            }
        }
    }

    companion object {
        private const val PERMISSION_REQUEST_ACCESS_FINE_LOCATION = 100
    }
}
